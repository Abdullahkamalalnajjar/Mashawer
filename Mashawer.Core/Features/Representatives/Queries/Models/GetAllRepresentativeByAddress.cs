namespace Mashawer.Core.Features.Representatives.Queries.Models
{
    public class GetAllRepresentativeByAddress : IRequest<Response<IEnumerable<RepresentativeDTO>>>
    {
        public string Address { get; set; } = string.Empty;
        public GetAllRepresentativeByAddress(string address)
        {
            Address = address;
        }
    }
}
