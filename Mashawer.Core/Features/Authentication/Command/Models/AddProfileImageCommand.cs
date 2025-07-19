namespace Mashawer.Core.Features.Authentication.Command.Models
{
    public class AddProfileImageCommand : IRequest<Response<string>>
    {
        public string UserId { get; set; }
        public IFormFile Image { get; set; } 
    }
}
