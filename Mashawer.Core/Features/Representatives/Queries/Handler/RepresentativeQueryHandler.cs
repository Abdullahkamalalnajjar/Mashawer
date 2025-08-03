﻿using Mashawer.Core.Features.Representatives.Queries.Models;

namespace Mashawer.Core.Features.Representatives.Queries.Handler
{
    public class RepresentativeQueryHandler(IRepresentativeService representativeService) : ResponseHandler,
        IRequestHandler<GetAllRepresentativeByAddress, Response<IEnumerable<RepresentativeDTO>>>,
        IRequestHandler<GetNearestRepresentativeQuery, Response<IEnumerable<NearestRepresentativeDto>>>
    {
        private readonly IRepresentativeService _representativeService = representativeService;
        public async Task<Response<IEnumerable<RepresentativeDTO>>> Handle(GetAllRepresentativeByAddress request, CancellationToken cancellationToken)
        {
            var representatives = await _representativeService.GetAllRepresentativesByAddressAsync(request.Address);
            return Success(representatives, "Representatives retrieved successfully.");
        }

        public async Task<Response<IEnumerable<NearestRepresentativeDto>>> Handle(GetNearestRepresentativeQuery request, CancellationToken cancellationToken)
        {
            var nearestRepresentatives = await _representativeService.GetNearestRepresentativeAsync(request.FromLatitude, request.FromLongitude, request.ToLatitude, request.ToLongitude);
            if (nearestRepresentatives == null || !nearestRepresentatives.Any())
            {
                return NotFound<IEnumerable<NearestRepresentativeDto>>("No nearest representatives found.");
            }
            return Success(nearestRepresentatives, "Nearest representatives retrieved successfully.");
        }
    }
}
