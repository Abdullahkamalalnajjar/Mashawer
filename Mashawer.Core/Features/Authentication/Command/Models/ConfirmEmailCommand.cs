namespace Mashawer.Core.Features.Authentication.Command.Models
{
    public class ConfirmEmailCommand : IRequest<Response<string>>
    {
        public string UserId { get; set; }
        public string Code { get; set; }
    }

}
