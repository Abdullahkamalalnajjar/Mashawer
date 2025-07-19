using Mashawer.Core.Features.GoogleService.Model;

namespace Mashawer.Core.Features.GoogleService.Handler
{
    public class GoogleHandler(IGoogleService googleService) : ResponseHandler,
        IRequestHandler<GoogleSignInCommand, Response<AuthResponse>>
    {
        private readonly IGoogleService _googleService = googleService;

        public async Task<Response<AuthResponse>> Handle(GoogleSignInCommand request, CancellationToken cancellationToken)
        {
            var result =await _googleService.GoogleLogin(request.IdToken, cancellationToken);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                if (result.ErrorMessage == "EmailNotConfirmed")
                    return BadRequest<AuthResponse>("Email is not confirmed. Please confirm your email first.");
                return BadRequest<AuthResponse>(result.ErrorMessage);
            }

            return Success(result.Response!, "Login successful");
        }
    }
}
