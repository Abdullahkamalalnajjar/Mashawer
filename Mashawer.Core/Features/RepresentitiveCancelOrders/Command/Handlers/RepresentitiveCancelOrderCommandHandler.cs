using Mashawer.Core.Features.RepresentitiveCancelOrders.Command.Models;
using Mashawer.Core.Helpers;
using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.RepresentitiveCancelOrders.Command.Handlers
{
    public class RepresentitiveCancelOrderCommandHandler : ResponseHandler, IRequestHandler<AddRepresentitiveCancelOrderCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepresentitiveCancelOrderService _representitiveCancelOrderService;
        private readonly IOrderService _orderService;

        public RepresentitiveCancelOrderCommandHandler(IUnitOfWork unitOfWork, IRepresentitiveCancelOrderService representitiveCancelOrderService, IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _representitiveCancelOrderService = representitiveCancelOrderService;
            _orderService = orderService;
        }

        public async Task<Response<string>> Handle(AddRepresentitiveCancelOrderCommand request, CancellationToken cancellationToken)
        {
            string? currentDriverId = null;

            var order = await _unitOfWork.Orders
                .GetTableAsTracking()
                .Include(x => x.Driver)
                .Include(x => x.Client)
                .Include(x => x.Tasks)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order == null)
                return NotFound<string>("Order not found.");

            currentDriverId = order.DriverId;
            if (string.IsNullOrWhiteSpace(currentDriverId))
                return BadRequest<string>("Order is not assigned to a representative.");

            if (OrderCancellationWalletHelper.ShouldApplyCancellationFee(order))
            {
                await OrderCancellationWalletHelper.AdjustWalletBalanceAsync(
                    _unitOfWork,
                    currentDriverId,
                    -OrderCancellationWalletHelper.CancellationFeeAmount,
                    "CancellationFee",
                    order.Id,
                    cancellationToken);
            }

            // ❌ مش Pending … لازم تكون CancelledByDriver
            order.Status = OrderStatus.Pending;
            order.CancelReason = request.Reason;
            if (order.Driver != null)
                order.AddressDriverAfterCancel = order.Driver.RepresentativeAddress;
            order.DriverId = null;
            order.Driver = null;
            // تحديث Tasks
            foreach (var orderTask in order.Tasks)
            {
                orderTask.Status = OrderStatus.Pending;
            }

            _unitOfWork.Orders.Update(order);

            var representitiveCancelOrder = new RepresentitiveCancelOrder
            {
                UserId = currentDriverId,
                OrderId = request.OrderId,
                Reason = request.Reason
            };

            var result = await _representitiveCancelOrderService.AddAsync(representitiveCancelOrder);

            if (result == "Created")
            {
                await _unitOfWork.CompeleteAsync();
                return Success<string>("Representative cancelled the order successfully.");
            }

            return BadRequest<string>("Failed to create representative cancel order.");
        }

    }
}
