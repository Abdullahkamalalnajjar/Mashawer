namespace Mashawer.Core.Features.Authentication.Command.Models
{
    public class CreateNewRefreshTokenCommand : IRequest<Response<AuthResponse>>
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }

    }
}
