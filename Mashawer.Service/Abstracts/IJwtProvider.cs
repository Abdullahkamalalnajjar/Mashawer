namespace Mashawer.Service.Abstracts
{
    public interface IJwtProvider

    {
        (string token, int expiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> claims);
        string ValidateToken(string token);
    }
}
