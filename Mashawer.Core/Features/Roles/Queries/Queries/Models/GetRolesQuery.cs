namespace Mashawer.Core.Features.Roles.Queries.Queries.Models
{
    public class GetRolesQuery : IRequest<Response<IEnumerable<RoleResponse>>>
    {
        public bool? IncludeDeleted { get; set; }
    }
}
