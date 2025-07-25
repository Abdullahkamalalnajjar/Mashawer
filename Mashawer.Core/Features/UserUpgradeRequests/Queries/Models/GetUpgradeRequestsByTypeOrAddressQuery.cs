using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.UserUpgradeRequests.Queries.Models
{
    public class GetUpgradeRequestsByTypeOrAddressQuery : IRequest<Response<IEnumerable<UserUpgradeRequestResponse>>>
    {
        public string? Address { get; set; }
        public RequestedRole RequestedRole { get; set; }
        public GetUpgradeRequestsByTypeOrAddressQuery(string? address, RequestedRole requestedRole)
        {
            Address = address;
            RequestedRole = requestedRole;
        }
    }
}
