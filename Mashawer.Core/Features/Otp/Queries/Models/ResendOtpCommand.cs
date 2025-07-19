namespace Mashawer.Core.Features.Otp.Queries.Models
{
    public class ResendOtpCommand : IRequest<Response<string>>
    {
        public string Email { get; set; }
    }
}
