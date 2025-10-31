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

            // تطبيق خصم التطبيق (لو موجود)
            if (generalSetting != null && generalSetting.DiscountPercentage > 0)
            {
                order.DeliveryPrice -= order.DeliveryPrice * generalSetting.DiscountPercentage;
            }

            // التعامل مع الدفع (مثلاً لو Paymob)
            if (order.PaymentMethod == PaymentMethod.Visa || order.PaymentMethod == PaymentMethod.LocalWallet)
            {
                order.PaymentStatus = PaymentStatus.Pending; // لسه الدفع تحت المعالجة
            }
            order.CalcTotalPrice();

            // إنشاء الطلب
            var result = await _orderService.CreateOrderAsync(order, cancellationToken);

            if (result == "Created")
            {
                await _unitOfWork.CompeleteAsync();
                return Created("Order has been created successfully");
            }

            return UnprocessableEntity<string>("An error occurred while creating the order");
        }

        public async Task<Response<string>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {

            var order = await _unitOfWork.Orders.GetTableAsTracking().Where(x => x.Id == request.OrderId).Include(c => c.Client).FirstOrDefaultAsync();

            if (order == null)
                return NotFound<string>("Order not found");

            // ✅ لو السواق وافق، لازم نضيف DriverId
            if (request.NewStatus == OrderStatus.Confirmed)
            {
                if (string.IsNullOrEmpty(request.DriverId))
                    return BadRequest<string>("DriverId is required for confirmation.");

                var driver = await userManager.FindByIdAsync(request.DriverId);

                if (driver == null)
                    return BadRequest<string>("Driver not found.");

                order.DriverId = driver.Id;

            }
            // if order already confirmed
            if (order.Status == OrderStatus.Confirmed && request.NewStatus == OrderStatus.Confirmed)
            {
                return BadRequest<string>("Order Already confirmed.");
            }
            order.Status = request.NewStatus;
            /* if (!string.IsNullOrEmpty(order.Client.FCMToken))
             {
                 await _notificationService.SendNotificationAsync(
                     userId: order.ClientId,
                     fcmToken: order.Client.FCMToken,
                     title: request.NewStatus == OrderStatus.Confirmed ? "Order Confirmed" : "Order Cancelled",
                     body: request.NewStatus == OrderStatus.Confirmed ? "Your order has been confirmed by a driver." : "Your order has been cancelled.",
                     cancellationToken: cancellationToken,
                     orderId: order.Id
                 );
             }*/
            await _unitOfWork.CompeleteAsync();

            return Success("Order status updated successfully");


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

