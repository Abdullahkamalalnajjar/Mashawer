using Mashawer.Core.Features.Otp.Queries.Models;

namespace Mashawer.Core.Features.Otp.Queries.Handlers
{
    public class VerifyOtpCommandHandler : ResponseHandler, IRequestHandler<VerifyOTpCommand, Response<string>>,
        IRequestHandler<ResendOtpCommand, Response<string>>
    {
        private readonly IOtpService _otpService;
        private readonly UserManager<ApplicationUser> _userManager;

        public VerifyOtpCommandHandler(IOtpService otpService, UserManager<ApplicationUser> userManager)
        {
            _otpService = otpService;
            _userManager = userManager;
        }

        public async Task<Response<string>> Handle(VerifyOTpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return BadRequest<string>("User not found");

            var isValid = await _otpService.VerifyOtpAsync(request.Email, request.Otp);
            if (!isValid)
                return BadRequest<string>("Invalid OTP or OTP expired");

            if (!user.EmailConfirmed)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest<string>(errors);
                }
            }

            await _otpService.SendOtpVerifiedSuccessAsync(user.Email, user.FullName ?? user.Email);
            return  Success<string>("Email verified successfully");
        }

        public async Task<Response<string>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return (BadRequest<string>("User not found"));
            var otpResponse = await _otpService.ResendOtpAsync(request.Email);
            if (!otpResponse.Success)
                return (BadRequest<string>(otpResponse.Message));
            return (Success<string>("OTP resent successfully"));
        }
    }
}