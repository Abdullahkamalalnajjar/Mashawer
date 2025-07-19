
namespace Mashawer.Core.Features.Users.Queries.Models
{
    public class UserProfileQuery : IRequest<Response<UserProfileResponse>>
    {
        public string UserId { get; set; }
    }
}
