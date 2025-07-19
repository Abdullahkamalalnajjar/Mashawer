using Google.Apis.Auth;

namespace Mashawer.Service.Abstracts
{
    public interface IGoogleService
    {
        public Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string idToken);
        public Task<AuthResult> GoogleLogin(string idToken, CancellationToken cancellationToken);

    }
}
