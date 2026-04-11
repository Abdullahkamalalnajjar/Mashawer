namespace Mashawer.Core.Features.Users.Queries.Models
{
    public class GetAllUserQuery : IRequest<Response<IEnumerable<UserResponse>>>
    {
        public string? Address { get; set; }

        public GetAllUserQuery(string? address)
        {
            Address = address;
        }
    }
}
