namespace Mashawer.Data.Dtos
{
    public record AddRoleRequest
    (string Name, IList<string> Permissions);

}
