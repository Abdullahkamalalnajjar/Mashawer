using Mashawer.Core.Features.Orders.Commands.Models;
using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Core.Features.Orders.Commands.Handler
{
    public class OrderCommandHandler(IOrderService orderService, IMapper mapper, IUnitOfWork unitOfWork, UserManager<ApplicationUser> _userManager , IHttpContextAccessor httpContextAccessor) : ResponseHandler,
        IRequestHandler<CreateOrderCommand, Response<string>>,
        IRequestHandler<UpdateOrderStatusCommand, Response<string>>,
        IRequestHandler<AddOrderPhotosCommand, Response<string>>
    {
        private readonly IOrderService _orderService = orderService;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager = _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<Response<string>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<Order>(request);
            var result = await _orderService.CreateOrderAsync(order, cancellationToken);
            if (result == "Created")
            {
                await _unitOfWork.CompeleteAsync();
                return Created("Order has been created");
            }
            return UnprocessableEntity<string>("Exist error when make order");
        }

        public async Task<Response<string>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            //var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);

            //if (order == null)
            //    return NotFound<string>("Order not found");

            //if (order.Status == OrderStatus.Completed)
            //    return BadRequest<string>("Cannot modify a completed order.");

            //if (request.NewStatus != OrderStatus.Confirmed && request.NewStatus != OrderStatus.Cancelled)
            //    return BadRequest<string>("Invalid status update. Allowed: Confirmed or Cancelled.");

            //order.Status = request.NewStatus;
            //await _unitOfWork.CompeleteAsync();

            //return Success("Order status updated successfully");

            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);

            if (order == null)
                return NotFound<string>("Order not found");

            if (order.Status == OrderStatus.Completed)
                return BadRequest<string>("Cannot modify a completed order.");

            if (request.NewStatus != OrderStatus.Confirmed && request.NewStatus != OrderStatus.Cancelled)
                return BadRequest<string>("Invalid status update. Allowed: Confirmed or Cancelled.");

            // ✅ تحقق إن الأوردر لسه Pending قبل ما يتم التأكيد
            if (order.Status != OrderStatus.Pending)
                return BadRequest<string>("Order already accepted by another driver.");

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

            order.Status = request.NewStatus;
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

