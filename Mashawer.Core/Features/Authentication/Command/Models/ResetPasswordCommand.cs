namespace Mashawer.Core.Features.Authentication.Command.Models
{
    public class ResetPasswordCommand : IRequest<Response<string>>
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
