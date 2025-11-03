using Mashawer.Core.Features.Representatives.Command.Models;

namespace Mashawer.Core.Features.Representatives.Command.Handler
{
    public class RepresentativeCommandHandler(IRepresentativeService representativeService) : ResponseHandler,
        IRequestHandler<UpdateRepresentativesLocationCommand, Response<string>>,
        IRequestHandler<UpdateRepresentativeInfoCommand, Response<string>>,
        IRequestHandler<MarkIsClientLateCommand, Response<string>>
    {
        private readonly IRepresentativeService _representativeService = representativeService;

        public async Task<Response<string>> Handle(UpdateRepresentativesLocationCommand request, CancellationToken cancellationToken)
        {
            var result = await _representativeService.UpdateLocation(request.UserId, request.RepresentativeLatitude, request.RepresentativeLongitude);
            if (result == "NotFound")
                return NotFound<string>("NotFound");
            return Success<string>(result);

        }
        public async Task<Response<string>> Handle(UpdateRepresentativeInfoCommand request, CancellationToken cancellationToken)
        {
            var result = await _representativeService.UpdateInfo(request.RepresentativeId, request.VehicleUrl!, request.VehicleNumber!, request.VehicleType!);
            if (result == "NotFound")
                return (NotFound<string>("NotFound"));
            return (Success<string>(result));
        }

        public async Task<Response<string>> Handle(MarkIsClientLateCommand request, CancellationToken cancellationToken)
        {
            var res = await _representativeService.MarkIsClientLate(request.OrderId);
            if (res == "NotFound")
                return NotFound<string>("NotFound");
            return Updated<string>(res);
        }
    }

}
