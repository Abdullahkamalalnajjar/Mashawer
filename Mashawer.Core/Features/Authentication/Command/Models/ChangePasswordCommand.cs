namespace Mashawer.Core.Features.Users.Commands.Models
{
    public class ChangePasswordCommand : IRequest<Response<string>>
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

}
