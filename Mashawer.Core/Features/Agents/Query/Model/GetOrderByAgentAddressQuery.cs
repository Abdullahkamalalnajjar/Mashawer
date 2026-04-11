using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.Agents.Query.Model
{
    public class GetOrderByAgentAddressQuery : IRequest<Response<List<OrderDto>>>
    {
        public GetOrderByAgentAddressQuery(string userId, OrderStatus orderStatus, DateTime? dateTime)
        {
            UserId = userId;
            OrderStatus = orderStatus;
            DateTime = dateTime;
        }

        public string UserId { get; set; }
        public DateTime? DateTime { get; set; }
        public OrderStatus OrderStatus
        {
            get; set;
        }
    }
}