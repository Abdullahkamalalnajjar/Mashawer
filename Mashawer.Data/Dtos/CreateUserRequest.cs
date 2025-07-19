namespace Mashawer.Data.Dtos
{
    public record CreateUserRequest
    (string Email, string FirstName, string LastName, string Password, IList<string> Roles);
}
