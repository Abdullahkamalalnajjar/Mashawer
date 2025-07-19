namespace Mashawer.Core.Features.UserUpgradeRequests.Queries.Models
{
    public class GetUserUpgradeRequestByUserIdQuery : IRequest<Response<IEnumerable<UserUpgradeRequestResponse>>>
    {
        public string UserId { get; set; } = null!;
        public GetUserUpgradeRequestByUserIdQuery(string userId)
        {
            UserId = userId;
        }
    }
}
