namespace Mashawer.Core.Features.Users.Commands.Models
{
    public class DeleteUserWithReasonCommand : IRequest<Response<string>>
    {
         public string Reason { get; set; }
    }
}
