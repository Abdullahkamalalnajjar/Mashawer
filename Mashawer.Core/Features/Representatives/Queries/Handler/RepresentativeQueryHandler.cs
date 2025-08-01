using Mashawer.Core.Features.Representatives.Queries.Models;

namespace Mashawer.Core.Features.Representatives.Queries.Handler
{
    public class RepresentativeQueryHandler(IRepresentativeService representativeService) : ResponseHandler,
        IRequestHandler<GetAllRepresentativeByAddress, Response<IEnumerable<RepresentativeDTO>>>
    {
        private readonly IRepresentativeService _representativeService = representativeService;
        public async Task<Response<IEnumerable<RepresentativeDTO>>> Handle(GetAllRepresentativeByAddress request, CancellationToken cancellationToken)
        {
            var representatives = await _representativeService.GetAllRepresentativesByAddressAsync(request.Address);
            return Success(representatives, "Representatives retrieved successfully.");
        }
    }
}
