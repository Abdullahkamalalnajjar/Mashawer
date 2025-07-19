namespace Mashawer.Core.Features.Authentication.Command.Models
{
    public class SendOtpCommand : IRequest<Response<string>>
    {
        public string Email { get; set; }
    }
}
