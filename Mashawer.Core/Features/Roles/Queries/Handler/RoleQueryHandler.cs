
namespace Mashawer.Core.Features.Roles.Queries.Handler
{
    public class RoleQueryHandler(IRoleService roleService) : ResponseHandler,
        IRequestHandler<GetRolesQuery, Response<IEnumerable<RoleResponse>>>,
        IRequestHandler<GetRolesDetailsQuery, Response<RoleDetailsResponse>>
    {
        private readonly IRoleService _roleService = roleService;

        public async Task<Response<IEnumerable<RoleResponse>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var result = await _roleService.GetAllRolesAsync(request.IncludeDeleted);
            return Success(result);
        }

        public async Task<Response<RoleDetailsResponse>> Handle(GetRolesDetailsQuery request, CancellationToken cancellationToken)
        {
            var response = await _roleService.GetRolesDetailsAsync(request.RoleId);
            return Success(response);
        }
    }
}
