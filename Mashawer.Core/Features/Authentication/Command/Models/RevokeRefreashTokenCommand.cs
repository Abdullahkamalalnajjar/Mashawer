namespace Mashawer.Core.Features.Authentication.Command.Models
{
    public class RevokeRefreashTokenCommand : IRequest<Response<string>>
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }

}
