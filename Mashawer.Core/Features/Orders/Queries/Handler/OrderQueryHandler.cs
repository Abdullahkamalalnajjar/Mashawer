using Mashawer.Core.Features.Orders.Queries.Models;

namespace Mashawer.Core.Features.Orders.Queries.Handler
{
    public class OrderQueryHandler(IOrderService orderService) : ResponseHandler,
        IRequestHandler<GetOrdersQuery, Response<IEnumerable<OrderDto>>>
    {
        private readonly IOrderService _orderService = orderService;

        public async Task<Response<IEnumerable<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetOrdersAsync();
            return Success(orders);
        }
    }
}
