namespace Mashawer.Data.Helpers
{
    public record AuthResponse
    (
        string Id,
        string? Email,
        string FirstName,
        string LastName,
        string Token,
        bool IsDisable,
        int TokenExpiresIn,
        string? RefreshToken,
        string UserType,
        DateTime RefreshTokenExpiresIn
 );
}
