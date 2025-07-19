namespace Mashawer.Core.Features.Users.Commands.Models
{
    public class CreateUserCommand : IRequest<Response<string>>
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

    }
}
