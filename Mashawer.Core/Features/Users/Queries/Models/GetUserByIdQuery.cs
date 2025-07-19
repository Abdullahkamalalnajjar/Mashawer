namespace Mashawer.Core.Features.Users.Queries.Models
{
    public class GetUserByIdQuery : IRequest<Response<UserResponse>>
    {
        public string UserId { get; set; }
    }
}
