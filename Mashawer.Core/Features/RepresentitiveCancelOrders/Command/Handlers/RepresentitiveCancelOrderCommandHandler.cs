using Mashawer.Core.Features.RepresentitiveCancelOrders.Command.Models;
using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.Core.Features.RepresentitiveCancelOrders.Command.Handlers
{
    public class RepresentitiveCancelOrderCommandHandler:  ResponseHandler, IRequestHandler<AddRepresentitiveCancelOrderCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepresentitiveCancelOrderService _representitiveCancelOrderService;
        private readonly IOrderService _orderService;

    public RepresentitiveCancelOrderCommandHandler(IUnitOfWork unitOfWork, IRepresentitiveCancelOrderService  representitiveCancelOrderService, IOrderService orderService)
    {
        _unitOfWork = unitOfWork;
        _representitiveCancelOrderService = representitiveCancelOrderService;
        _orderService = orderService;
    }

        public async Task<Response<string>> Handle(AddRepresentitiveCancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders
                .GetTableAsTracking()
                .Include(x => x.Driver)
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId);

            if (order == null)
                return NotFound<string>("Order not found.");

            // ✅ لازم نتأكد أن حالة الطلب مؤهلة للإلغاء من قبل السائق
            if (order.Status != OrderStatus.Confirmed)
                return BadRequest<string>("The order cannot be cancelled because it is not confirmed.");

            // حفظ DriverId قبل ما يتعمل null
            var driverId = order.DriverId;

            // ❌ مش Pending … لازم تكون CancelledByDriver
            order.Status = OrderStatus.Pending;

            // تفريغ السائق
            order.DriverId = null;

            // تحديث Tasks
            foreach (var orderTask in order.Tasks)
            {
                orderTask.Status = OrderStatus.Cancelled;
            }

            _unitOfWork.Orders.Update(order);

            var representitiveCancelOrder = new RepresentitiveCancelOrder
            {
                RepresentitiveId = driverId,
                OrderId = request.OrderId,
                CancelReason = request.Reason
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
