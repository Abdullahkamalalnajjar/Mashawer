using Mashawer.Core.Features.Orders.Commands.Models;
using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.Orders.Commands.Handler
{
    public class OrderCommandHandler(IOrderService orderService, INotificationService notificationService, IMapper mapper, IUnitOfWork unitOfWork, UserManager<ApplicationUser> _userManager, IHttpContextAccessor httpContextAccessor) : ResponseHandler,
        IRequestHandler<CreateOrderCommand, Response<string>>,
        IRequestHandler<UpdateOrderStatusCommand, Response<string>>,
        IRequestHandler<AddOrderPhotosCommand, Response<string>>
    {
        private readonly IOrderService _orderService = orderService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager = _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<Response<string>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var generalSetting = await _unitOfWork.GeneralSettings
                .GetTableNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            // تحويل الـ Command إلى Entity
            var order = _mapper.Map<Order>(request);

            // ✅ حساب المسافة وسعر التوصيل
            order.DistanceKm = Math.Round(GeoHelper.CalculateDistance(
                order.FromLatitude, order.FromLongitude,
                order.ToLatitude, order.ToLongitude), 2);

            order.DeliveryPrice = GeoHelper.CalculateDeliveryPrice(
                order.FromLatitude, order.FromLongitude,
                order.ToLatitude, order.ToLongitude);

            // تطبيق خصم التطبيق (لو موجود)
            if (generalSetting != null && generalSetting.DiscountPercentage > 0)
            {
                order.DeducationDelivery = order.DeliveryPrice * generalSetting.DiscountPercentage;
            }

            // التعامل مع الدفع
            if (order.PaymentMethod == PaymentMethod.Visa || order.PaymentMethod == PaymentMethod.LocalWallet)
                order.PaymentStatus = PaymentStatus.Pending;

            // حساب الإجمالي (المشتريات + التوصيل)
            order.CalcTotalPrice();

            // إنشاء الطلب
            var result = await _orderService.CreateOrderAsync(order, cancellationToken);

            if (result == "Created")
            {
                await _unitOfWork.CompeleteAsync();
                return Created("Order has been created successfully", new { orderId = order.Id });
            }

            return UnprocessableEntity<string>("An error occurred while creating the order");
        }


        public async Task<Response<string>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            // ✅ جلب الطلب مع العميل والسائق (لو موجود)
            var order = await _unitOfWork.Orders
                .GetTableAsTracking()
                .Include(o => o.Client)
                .Include(o => o.Driver)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
                return NotFound<string>("Order not found.");

            // ✅ لو الطلب بالفعل في نفس الحالة المطلوبة
            if (order.Status == request.NewStatus)
                return BadRequest<string>($"Order is already {order.Status}.");

            // ✅ لو السواق هو اللي بيأكد الطلب
            if (request.NewStatus == OrderStatus.Confirmed)
            {
                if (string.IsNullOrWhiteSpace(request.DriverId))
                    return BadRequest<string>("DriverId is required for confirmation.");

                var driver = await userManager.FindByIdAsync(request.DriverId);
                if (driver == null)
                    return BadRequest<string>("Driver not found.");

                // ربط الطلب بالسائق الجديد
                order.DriverId = driver.Id;
            }

            // ✅ تحديث الحالة
            order.Status = request.NewStatus;

            // ✅ إرسال إشعار (لو فعّلت الخدمة)
            
            if (!string.IsNullOrEmpty(order.Client?.FCMToken))
            {
                var title = request.NewStatus == OrderStatus.Confirmed
                    ? "Order Confirmed"
                    : request.NewStatus == OrderStatus.Cancelled
                        ? "Order Cancelled"
                        : "Order Updated";

                var body = request.NewStatus == OrderStatus.Confirmed
                    ? "Your order has been confirmed by a driver."
                    : request.NewStatus == OrderStatus.Cancelled
                        ? "Your order has been cancelled."
                        : $"Order status changed to {request.NewStatus}.";

                await _notificationService.SendNotificationAsync(
                    userId: order.ClientId,
                    fcmToken: order.Client.FCMToken,
                    title: title,
                    body: body,
                    cancellationToken: cancellationToken,
                    orderId: order.Id
                );
            }
            

            // ✅ حفظ التغييرات
            await _unitOfWork.CompeleteAsync();

            return Success($"Order status updated to {request.NewStatus} successfully.");
        }


        async Task<Response<string>> IRequestHandler<AddOrderPhotosCommand, Response<string>>.Handle(AddOrderPhotosCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
            if (order == null)
                return new Response<string>("Order not found");

            if (request.ItemPhotoBefore != null)
                order.ItemPhotoBefore = FileHelper.SaveFile(request.ItemPhotoBefore, "OrderPhotos", _httpContextAccessor);

            if (request.ItemPhotoAfter != null)
                order.ItemPhotoAfter = FileHelper.SaveFile(request.ItemPhotoAfter, "OrderPhotos", _httpContextAccessor);

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompeleteAsync();

            return new Response<string>("Photos updated successfully", true);
        }
    }
}

