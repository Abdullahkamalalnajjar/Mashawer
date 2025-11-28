using Mashawer.Core.Features.ClientCancelOrders.Command.Model;
using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.ClientCancelOrders.Command.Handler
{
    public class AddClientCancelOrderCommandHandler : ResponseHandler, IRequestHandler<AddClientCancelOrderCommand, Response<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClientCancelOrderService _clientCancelOrderService;
        private readonly IOrderService _orderService;

        public AddClientCancelOrderCommandHandler(IUnitOfWork unitOfWork, IClientCancelOrderService clientCancelOrderService, IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _clientCancelOrderService = clientCancelOrderService;
            _orderService = orderService;
        }

        public async Task<Response<string>> Handle(AddClientCancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetTableAsTracking().Include(x => x.Driver).Include(x => x.Tasks).Include(x => x.Client).Where(x => x.Id == request.OrderId).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound<string>("Order not found.");
            }
            // update order status to cancelled by client
            order.Status = OrderStatus.Cancelled;
            // make order's driver id null
            order.DriverId = null;
            // make ordertask is cancelled
            foreach (var orderTask in order.Tasks)
            {
                orderTask.Status = OrderStatus.Cancelled;
            }
            // update order in database
            _unitOfWork.Orders.Update(order);

            var clientCancelOrder = new ClientCancelOrder
            {
                ClientId = order.ClientId,
                OrderId = request.OrderId,
                CancelReason = request.Reason
            };


            var result = await _clientCancelOrderService.AddAsync(clientCancelOrder);


            if (result == "Created")
            {
                await _unitOfWork.CompeleteAsync();
                return Success<string>("Client cancel order created successfully.");
            }

            return BadRequest<string>("Failed to create client cancel order.");
        }
    }
}