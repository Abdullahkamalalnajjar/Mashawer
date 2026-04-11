namespace Mashawer.Core.Features.Authentication.Command.Models
{
    public class CompleteProfileCommand : IRequest<Response<object>>
    {
        public string UserId { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Address { get; set; } = null!;
    }
}
