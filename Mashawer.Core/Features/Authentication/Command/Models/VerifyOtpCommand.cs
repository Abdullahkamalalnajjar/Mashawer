namespace Mashawer.Core.Features.Authentication.Command.Models
{
    public class VerifyOtpCommand : IRequest<Response<string>>
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
