using Mashawer.Core.Features.Orders.Commands.Models;
using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;
using Mashawer.Data.Dtos;
using Mashawer.Service.Abstracts;
using Mashawer.Data.Helpers;


namespace Mashawer.Core.Features.Orders.Commands.Handler
{
    public class OrderCommandHandler(
        IOrderService orderService,
        INotificationService notificationService,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IUserDailyDiscountService userDailyDiscountService,
        IPaymobService paymobService,
        IWalletService walletService,
        ICurrentUserService currentUserService
    ) : ResponseHandler,
        IRequestHandler<CreateOrderCommand, Response<string>>,
        IRequestHandler<UpdateOrderStatusCommand, Response<string>>,
        IRequestHandler<AddOrderTaskPhotosCommand, Response<string>>
    {
        private readonly IOrderService _orderService = orderService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IUserDailyDiscountService _userDailyDiscountService = userDailyDiscountService;
        private readonly IPaymobService _paymobService = paymobService;
        private readonly IWalletService _walletService = walletService;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private const decimal RepresentativeConfirmationFeePercentage = 0.25m;
        private const decimal RepresentativeWalletMinBalance = -15m;

        public async Task<Response<string>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound<string>("User not found");
            }

            // 1) جلب إعدادات التطبيق
            var generalSetting = await _unitOfWork.GeneralSettings
                .GetTableNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            // 2) تحويل الـ Command إلى Entity
            var order = _mapper.Map<Order>(request);

            // 3) تطبيق خصم التطبيق العام
            decimal appDiscount = 0;
            if (generalSetting != null && generalSetting.DiscountPercentage > 0)
            {
                appDiscount = order.TotalPrice.GetValueOrDefault() * generalSetting.DiscountPercentage;
                order.DeducationDelivery = appDiscount;
            }

            // 4) تطبيق الخصم اليومي للعميل
            decimal dailyDiscount = 0;
            var userDiscount = await _userDailyDiscountService
                .GetUserDiscountAsync(order.ClientId);

            if (userDiscount != null)
            {
                dailyDiscount = userDiscount.Sum(d => d.DiscountAmount);
                order.DeducationDelivery = (order.DeducationDelivery ?? 0) + dailyDiscount;

                // منع إعادة استخدام الخصم
                await _userDailyDiscountService.MarkUsedAsync(userDiscount.Select(d => d.Id).ToList());
            }

            // 5) حساب السعر النهائي بعد الخصومات
            order.FinalPrice = (order.TotalPrice ?? 0) - (order.DeducationDelivery ?? 0);

            // 6) Handle payment logic
            if (request.PaymentMethod == PaymentMethod.AppWallet)
            {
                var wallet = await _walletService.GetWalletByUserIdAsync(user.Id);
                if (wallet == null || wallet.Balance < order.FinalPrice)
                {
                    return BadRequest<string>("رصيد غير كافي");
                }

                await _walletService.UpdateWalletBalanceAsync(wallet.Id, order.FinalPrice.Value, "OrderPayment",
                    cancellationToken);
                order.PaymentStatus = PaymentStatus.Paid;
            }
            else if (request.PaymentMethod == PaymentMethod.Visa)
            {
                order.Status = OrderStatus.Processing;
                order.PaymentStatus = PaymentStatus.Pending;
            }
            else
            {
                order.PaymentStatus = PaymentStatus.NotPaid;
            }

            // 7) Save the order
            var result = await _orderService.CreateOrderAsync(order, cancellationToken);
            if (result != "Created")
            {
                return UnprocessableEntity<string>("An error occurred while creating the order");
            }
            await _unitOfWork.CompeleteAsync();

            var createdOrder = await _orderService.GetOrderByIdAsync(order.Id);

            // 8) Post-save actions
            if (request.PaymentMethod == PaymentMethod.Visa)
            {
                var billingData = new PaymobDto.PaymobBillingData
                {
                    FirstName = user.FullName,
                    LastName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };
                var paymentUrl = await _paymobService.InitCardPaymentAsync(new()
                {
                    AmountCents = (int)(order.FinalPrice * 100),
                    MerchantOrderId = order.Id.ToString()
                });
                return Success(paymentUrl.IframeUrl, createdOrder);
            }
            
            return Created("Order has been created successfully", createdOrder);
        }

        // ✅ تحديث حالة الطلب
        public async Task<Response<string>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders
                .GetTableAsTracking()
                .Include(o => o.Client)
                .Include(o => o.Driver)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
                return NotFound<string>("Order not found.");

            if (order.Status == request.NewStatus)
                return BadRequest<string>($"Order is already {order.Status}.");

            if (request.NewStatus == OrderStatus.Cancelled && order.PaymentStatus == PaymentStatus.Paid)
            {
                if (order.FinalPrice.HasValue && order.FinalPrice.Value > 0)
                {
                    var wallet = await _walletService.GetWalletByUserIdAsync(order.ClientId);
                    if (wallet != null)
                    {
                        await _walletService.UpdateWalletBalanceAsync(wallet.Id, order.FinalPrice.Value, "Refund",
                            cancellationToken);
                        order.PaymentStatus = PaymentStatus.Refunded;
                    }
                }
            }

            if (request.NewStatus == OrderStatus.Confirmed)
            {
                if (string.IsNullOrWhiteSpace(request.DriverId))
                    return BadRequest<string>("DriverId is required for confirmation.");

                var driver = await _userManager.FindByIdAsync(request.DriverId);
                if (driver == null)
                    return BadRequest<string>("Driver not found.");

                var driverWallet = await _unitOfWork.Wallets
                    .GetTableAsTracking()
                    .FirstOrDefaultAsync(w => w.UserId == request.DriverId, cancellationToken);

                if (driverWallet == null)
                    return BadRequest<string>("محفظة المندوب غير موجودة");

                var orderAmount = order.FinalPrice ?? order.TotalPrice ?? 0m;
                var confirmationFee = orderAmount * RepresentativeConfirmationFeePercentage;
                var balanceAfterDeduction = driverWallet.Balance - confirmationFee;

                if (balanceAfterDeduction < RepresentativeWalletMinBalance)
                {
                    return BadRequest<string>("رصيد المندوب غير كافي لقبول الطلب");
                }

                driverWallet.Balance = balanceAfterDeduction;
                _unitOfWork.Wallets.Update(driverWallet);

                await _unitOfWork.WalletTransactions.AddAsync(new WalletTransaction
                {
                    WalletId = driverWallet.Id,
                    Amount = confirmationFee,
                    Type = "OrderFee",
                    Status = "Paid",
                    PaidAt = DateTime.UtcNow
                }, cancellationToken);

                order.DriverId = driver.Id;
            }

            order.Status = request.NewStatus;

            // ✅ إرسال إشعار للعميل
            if (!string.IsNullOrEmpty(order.Client?.FCMToken))
            {
                var (title, body) = request.NewStatus switch
                {
                    OrderStatus.Confirmed => ("Order Confirmed", "Your order has been confirmed by a driver."),
                    OrderStatus.Cancelled => ("Order Cancelled", "Your order has been cancelled."),
                    _ => ("Order Updated", $"Order status changed to {request.NewStatus}.")
                };

                await _notificationService.SendNotificationAsync(
                    userId: order.ClientId,
                    fcmToken: order.Client.FCMToken,
                    title: title,
                    body: body,
                    cancellationToken: cancellationToken,
                    orderId: order.Id
                );
            }

            await _unitOfWork.CompeleteAsync();
            return Success($"Order status updated to {request.NewStatus} successfully.");
        }

        // ✅ إضافة الصور
        async Task<Response<string>> IRequestHandler<AddOrderTaskPhotosCommand, Response<string>>.Handle(AddOrderTaskPhotosCommand request, CancellationToken cancellationToken)
        {
            var orderTask = await _unitOfWork.OrderTasks.GetByIdAsync(request.OrderTaskId);
            if (orderTask == null)
                return new Response<string>("Order not found");

            if (request.ItemPhotoBefore != null)
                orderTask.ItemPhotoBefore = FileHelper.SaveFile(request.ItemPhotoBefore, "OrderTaskPhotos", _httpContextAccessor);

            if (request.ItemPhotoAfter != null)
                orderTask.ItemPhotoAfter = FileHelper.SaveFile(request.ItemPhotoAfter, "OrderTaskPhotos", _httpContextAccessor);

            _unitOfWork.OrderTasks.Update(orderTask);
            await _unitOfWork.CompeleteAsync();

            return new Response<string>("Photos updated successfully", true);
        }

    }

}
