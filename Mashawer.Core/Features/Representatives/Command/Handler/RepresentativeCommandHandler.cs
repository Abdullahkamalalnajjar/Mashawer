using Mashawer.Core.Features.Representatives.Command.Models;
using Mashawer.Core.Features.Representatives.Queries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.Core.Features.Representatives.Command.Handler
{
    public class RepresentativeCommandHandler(IRepresentativeService representativeService) : ResponseHandler,
        IRequestHandler<UpdateRepresentativesLocationCommand, Response<string>>
    {
        private readonly IRepresentativeService _representativeService = representativeService;

        public async Task<Response<string>> Handle(UpdateRepresentativesLocationCommand request, CancellationToken cancellationToken)
        {
            var result = await _representativeService.UpdateLocation(request.UserId, request.RepresentativeLatitude, request.RepresentativeLongitude);
            if (result == "NotFound")
                return NotFound<string>("NotFound");
            return Success<string>(result); 
               
        }
    }

}
