namespace Mashawer.Core.Features.UserUpgradeRequests.Queries.Models
{
    public class GetAllUserUpgradeRequestByAddressQuery : IRequest<Response<IEnumerable<UserUpgradeRequestResponse>>>
    {
        public string Address { get; set; } = null!;
        public GetAllUserUpgradeRequestByAddressQuery(string address)
        {
            Address = address;
        }
    }
}
