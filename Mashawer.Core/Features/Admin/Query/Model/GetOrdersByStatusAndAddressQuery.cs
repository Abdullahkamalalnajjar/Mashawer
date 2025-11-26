using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.Admin.Query.Model
{
    public class GetOrdersByStatusAndAddressQuery : IRequest<Response<List<OrderDto>>>
    {
        public GetOrdersByStatusAndAddressQuery(OrderStatus orderStatus, string? address, DateTime? dateTime)
        {
            OrderStatus = orderStatus;
            Address = address;
            DateTime = dateTime;
        }

        public OrderStatus OrderStatus { get; set; }
        public string? Address { get; set; }
        public DateTime? DateTime { get; set; }
    }
}
