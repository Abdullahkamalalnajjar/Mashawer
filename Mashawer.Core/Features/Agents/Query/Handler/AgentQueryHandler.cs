using Mashawer.Core.Features.Agents.Query.Model;

namespace Mashawer.Core.Features.Agents.Query.Handler
{
    public class AgentQueryHandler(IAgentService agentService) : ResponseHandler,
        IRequestHandler<GetOrderByAgentAddressQuery, Response<List<OrderDto>>>
    {
        private readonly IAgentService _agentService = agentService;

        public async Task<Response<List<OrderDto>>> Handle(GetOrderByAgentAddressQuery request, CancellationToken cancellationToken)
        {
            var result = await _agentService.GetOrdersByAgentAddressAsync(request.UserId, request.DateTime);
            return Success(result);
        }
    }
}
