namespace Mashawer.Core.Features.UserUpgradeRequests.Queries.Models
{
    public class GetUserUpgradeRequestByAgentIdQuery : IRequest<Response<IEnumerable<UserUpgradeRequestResponse>>>
    {
        public string AgentId { get; set; } = null!;
        public GetUserUpgradeRequestByAgentIdQuery(string agentId)
        {
            AgentId = agentId;
        }
    }
}
