namespace Mashawer.Core.Features.Users.Commands.Models
{
    public class UnDisableUserCommand : IRequest<Response<string>>
    {
        public string UserId { get; set; }
    }
}
