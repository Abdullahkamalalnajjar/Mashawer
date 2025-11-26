using Mashawer.Core.Features.Orders.Commands.Models;
using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.Orders.Commands.Handler
{
    public class OrderCommandHandler(
        IOrderService orderService,
        INotificationService notificationService,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> _userManager,
        IHttpContextAccessor httpContextAccessor
    ) : ResponseHandler,
        IRequestHandler<CreateOrderCommand, Response<string>>,
        IRequestHandler<UpdateOrderStatusCommand, Response<string>>,
        IRequestHandler<AddOrderTaskPhotosCommand, Response<string>>
    {
        private readonly IOrderService _orderService = orderService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> userManager = _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<Response<string>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var generalSetting = await _unitOfWork.GeneralSettings
                .GetTableNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            // 🌀 تحويل الـ Command إلى Entity
            var order = _mapper.Map<Order>(request);

            double totalDistance = 0;
            decimal totalDeliveryPrice = 0;

            // ✅ جمع المسافات من Flutter وحساب السعر لكل مهمة
            foreach (var task in order.Tasks)
            {
                double distance = task.DistanceKm; // المسافة من Flutter
                totalDistance += distance;

                // ✅ سعر كل مهمة = المسافة بالكيلومتر (قابل للتعديل لاحقًا)

                task.DeliveryPrice = ((decimal)Math.Round(distance, 2) * 7) + 15;
            }

            // ✅ حساب إجمالي سعر التوصيل
            totalDeliveryPrice = (decimal)totalDistance * 7m;

            // ✅ تخزين الإجماليات داخل الطلب
            order.TotalDistanceKm = Math.Round(totalDistance, 2);
            order.TotalDeliveryPrice = Math.Round(totalDeliveryPrice, 2) + 15m;

            // ✅ تطبيق خصم التطبيق (إن وجد)
            if (generalSetting != null && generalSetting.DiscountPercentage > 0)
            {
                var discount = order.TotalDeliveryPrice * generalSetting.DiscountPercentage;
                order.DeducationDelivery = discount;
            }

            // ✅ تحديد حالة الدفع
            if (order.PaymentMethod == PaymentMethod.Visa || order.PaymentMethod == PaymentMethod.LocalWallet)
                order.PaymentStatus = PaymentStatus.Pending;

            // ✅ حساب السعر الإجمالي للطلب (السعر الكلي = سعر العناصر + سعر التوصيل)
            order.CalcTotalPrice();
            order.CalcTotalDeliveryPrice();
            order.TotalPrice = order.TotalPrice + order.TotalDeliveryPrice;

            // ✅ حفظ الطلب في قاعدة البيانات
            var result = await _orderService.CreateOrderAsync(order, cancellationToken);

            if (result == "Created")
            {
                await _unitOfWork.CompeleteAsync();
                return Created("Order has been created successfully", new { orderId = order.Id });
            }

            return UnprocessableEntity<string>("An error occurred while creating the order");
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

            if (request.NewStatus == OrderStatus.Confirmed)
            {
                if (string.IsNullOrWhiteSpace(request.DriverId))
                    return BadRequest<string>("DriverId is required for confirmation.");

                var driver = await userManager.FindByIdAsync(request.DriverId);
                if (driver == null)
                    return BadRequest<string>("Driver not found.");

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
