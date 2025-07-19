namespace Mashawer.Core.Features.Roles.Commands.Models
{
    public class CreateRoleWithPermissionsCommand : IRequest<Response<string>>
    {
        public string Name { get; set; }
        public IList<string> Permisstions { get; set; }
    }
}
