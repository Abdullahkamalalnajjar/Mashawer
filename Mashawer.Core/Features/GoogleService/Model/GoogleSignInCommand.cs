namespace Mashawer.Core.Features.GoogleService.Model
{
    public class GoogleSignInCommand : IRequest<Response<AuthResponse>>
    {
        public string IdToken { get; set; }
        public string FCMtoken { get; set; }
    }
}
