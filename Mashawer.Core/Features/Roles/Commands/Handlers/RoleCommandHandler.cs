
namespace Mashawer.Core.Features.Roles.Commands.Handlers
{
    public class RoleCommandHandler(IRoleService roleService) : ResponseHandler,
        IRequestHandler<CreateRoleWithPermissionsCommand, Response<string>>,
        IRequestHandler<EditRoleWithPermissionsCommand, Response<string>>
    {
        private readonly IRoleService _roleService = roleService;

        public async Task<Response<string>> Handle(CreateRoleWithPermissionsCommand request, CancellationToken cancellationToken)
        {
            var result = await _roleService.AddRoleWithPermissionAsync(request.Name, request.Permisstions);
            return result switch
            {
                "Created" => Created<string>("Role created succssfully"),
                "IsExist" => BadRequest<string>("Role is already exist"),
                "InvalidPermissions" => BadRequest<string>("Invalid Permissions inputs"),
                _ => UnprocessableEntity<string>("Some thing happen error")
            };
        }

        public async Task<Response<string>> Handle(EditRoleWithPermissionsCommand request, CancellationToken cancellationToken)
        {
            var result = await _roleService.UpdateRoleWithPermissionAsync(request.Id, request.Name, request.Permisstions);

            return result switch
            {
                "NotFound" => NotFound<string>("Role not existed"),
                "InvalidPermissions" => BadRequest<string>("Invalid permissions inputs"),
                "IsExist" => BadRequest<string>("Role is already exist"),
                "Updated" => Updated<string>("Role updated succssfully"),
                _ => UnprocessableEntity<string>("Some thing happen error")
            };
        }
    }
}
