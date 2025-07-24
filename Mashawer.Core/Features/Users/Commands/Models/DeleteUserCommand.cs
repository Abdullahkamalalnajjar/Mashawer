namespace Mashawer.Core.Features.Users.Commands.Models
{
    public class DeleteUserCommand : IRequest<Response<string>>
    {
        public string UserId { get; set; }

    }
}
