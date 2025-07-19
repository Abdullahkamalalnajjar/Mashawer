using Mashawer.Data.Helpers;

namespace Mashawer.Service.Abstracts
{
    public interface IAuthService
    {
        public Task<AuthResult> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
        public Task<AuthResponse> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
        public Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
        public Task<string> ConfirmEmailAsync(string userId, string code);
        public Task<string> ResendConfirmEmailAsync(string email);
    }
}
