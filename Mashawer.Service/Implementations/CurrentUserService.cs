namespace Mashawer.Service.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        public string? UserId { get; }
        public string? Username { get; }
        public string? Email { get; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;

            UserId = user?.FindFirst("sub")?.Value
                  ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Username = user?.FindFirst(ClaimTypes.Name)?.Value
                    ?? user?.Identity?.Name;

            Email = user?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}
