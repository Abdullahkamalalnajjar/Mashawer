namespace Mashawer.Core.Features.Authentication.Command.Models
{
    public class ResendConfirmEmailCommand : IRequest<Response<string>>
    {
        public string Email { get; set; }
    }
}
