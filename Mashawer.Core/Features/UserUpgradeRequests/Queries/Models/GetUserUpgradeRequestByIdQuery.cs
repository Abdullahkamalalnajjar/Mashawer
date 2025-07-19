namespace Mashawer.Core.Features.UserUpgradeRequests.Queries.Models
{
    public class GetUserUpgradeRequestByIdQuery : IRequest<Response<UserUpgradeRequest>>
    {
        public int Id { get; set; }
        public GetUserUpgradeRequestByIdQuery(int id)
        {
            Id = id;
        }
    }
}
