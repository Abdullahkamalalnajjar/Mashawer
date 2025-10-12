namespace Mashawer.Core.Features.Orders.Queries.Models
{
    public class GetOrderByIdQuery : IRequest<Response<OrderDto>>
    {
        public int OrderId { get; set; }
        public GetOrderByIdQuery(int orderId)
        {
            OrderId = orderId;
        }
    }
}
