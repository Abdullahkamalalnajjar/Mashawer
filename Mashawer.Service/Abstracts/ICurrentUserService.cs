namespace Mashawer.Service.Abstracts
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Username { get; }
        string? Email { get; }
    }

}
