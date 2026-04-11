namespace Mashawer.Core.Features.Users.Queries.Models
{
    public class GetAllRepresentativeQuery : IRequest<Response<IEnumerable<UserResponse>>>
    {
        public GetAllRepresentativeQuery(string? address)
        {
            Address = address;
        }

        public string? Address { get; set; }
    }
}
