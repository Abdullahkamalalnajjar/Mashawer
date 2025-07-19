namespace Mashawer.Data.Dtos
{
    public record RoleDetailsResponse
   (string Id, string Name, bool IsDeleted, IEnumerable<string> Permissions);
}
