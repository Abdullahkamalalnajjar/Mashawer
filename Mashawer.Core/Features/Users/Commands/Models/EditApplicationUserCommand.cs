namespace Mashawer.Core.Features.Users.Commands.Models
{
    public class EditApplicationUserCommand : IRequest<Response<string>>
    {
        public string UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
