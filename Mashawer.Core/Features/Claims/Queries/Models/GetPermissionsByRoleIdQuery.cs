
namespace Mashawer.Core.Features.Claims.Queries.Models
{
    public class GetPermissionsByRoleIdQuery : IRequest<Response<ClaimResponse>>
    {
        public string RoleId { get; set; }
    }
}
