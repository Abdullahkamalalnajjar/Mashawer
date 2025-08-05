using Mashawer.Core.Features.Orders.Queries.Models;

namespace Mashawer.Core.Features.Orders.Queries.Handler
{
    public class OrderQueryHandler(IOrderService orderService) : ResponseHandler,
        IRequestHandler<GetOrdersQuery, Response<IEnumerable<OrderDto>>>,
        IRequestHandler<GetOrdersByClientIdQuery, Response<IEnumerable<OrderDto>>>,
        IRequestHandler<GetOrdersByDriverIdQuery, Response<IEnumerable<OrderDto>>>
    {
        private readonly IOrderService _orderService = orderService;

        public async Task<Response<IEnumerable<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetOrdersAsync();
            return Success(orders);
        }

        public async Task<Response<IEnumerable<OrderDto>>> Handle(GetOrdersByClientIdQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetOrdersByClientIdAsync(request.ClientId);
            if (orders == null || !orders.Any())
            return NotFound<IEnumerable<OrderDto>>("No orders found OR Invald ID");
            return Success(orders);
        }

        public async Task<Response<IEnumerable<OrderDto>>> Handle(GetOrdersByDriverIdQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetOrdersByDriverIdAsync(request.DriverId);
            if (orders == null || !orders.Any())
             return NotFound<IEnumerable<OrderDto>>("No orders found OR Invald ID");
            return Success(orders);
        }
    }
}
