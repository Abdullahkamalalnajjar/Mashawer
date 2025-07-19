namespace Mashawer.Core.Features.Users.Commands.Models
{
    public class UpdateUserCommand : IRequest<Response<string>>
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
