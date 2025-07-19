namespace Mashawer.Core.Features.Users.Commands.Models
{
    public class DeleteUserCommand : IRequest<Response<string>>
    {
         public string Reason { get; set; }
    }
}
