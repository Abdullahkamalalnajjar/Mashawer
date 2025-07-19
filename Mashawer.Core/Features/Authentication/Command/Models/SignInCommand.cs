namespace Mashawer.Core.Features.Authentication.Command.Models
{
    public class SignInCommand : IRequest<Response<AuthResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }

    }
}
