namespace Mashawer.Core.Features.Roles.Queries.Queries.Models
{
    public class GetRolesDetailsQuery : IRequest<Response<RoleDetailsResponse>>
    {
        public string RoleId { get; set; }
    }

}
