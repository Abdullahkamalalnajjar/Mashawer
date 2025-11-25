namespace Mashawer.Core.Features.Agents.Query.Model
{
    public class GetOrderByAgentAddressQuery : IRequest<Response<List<OrderDto>>>
    {
        public GetOrderByAgentAddressQuery(string userId, DateTime? dateTime)
        {
            UserId = userId;
            DateTime = dateTime;
        }

        public string UserId { get; set; }
        public DateTime? DateTime { get; set; }
    }
}
