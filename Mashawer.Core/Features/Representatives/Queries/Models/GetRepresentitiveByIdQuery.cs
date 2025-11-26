namespace Mashawer.Core.Features.Representatives.Queries.Models
{
    public class GetRepresentitiveByIdQuery : IRequest<Response<RepresentativeInfoDto>>
    {
        public GetRepresentitiveByIdQuery(string representativeId)
        {
            RepresentativeId = representativeId;
        }

        public string RepresentativeId { get; set; }
    }
}
