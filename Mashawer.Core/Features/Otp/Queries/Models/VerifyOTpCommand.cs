namespace Mashawer.Core.Features.Otp.Queries.Models
{
    public class VerifyOTpCommand : IRequest<Response<string>>
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public VerifyOTpCommand(string email, string otp)
        {
            Email = email;
            Otp = otp;
        }
    }
}
