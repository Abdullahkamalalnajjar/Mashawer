namespace Mashawer.Core.Features.Roles.Commands.Models
{
    public class EditRoleWithPermissionsCommand : IRequest<Response<string>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<string> Permisstions { get; set; }
    }
}
