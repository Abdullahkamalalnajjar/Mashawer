using Mashawer.Api.Base;
using Mashawer.Core.Features.Representatives.Command.Models;
using Mashawer.Core.Features.Representatives.Queries.Models;
using Mashawer.Data.AppMetaData;
using Mashawer.Service.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{
    public class RepresentativeController(IRepresentativeService representativeService) : AppBaseController
    {
        private readonly IRepresentativeService _representativeService = representativeService;

        [HttpGet(Router.RepresentativeRouting.GetApprovedRepresentativesByAddress)]
        public async Task<IActionResult> GetRepresentativesByAddress(string address)
        {
            var query = new GetAllRepresentativeByAddress(address);
            var result = await Mediator.Send(query);
            return NewResult(result);
        }
        [HttpGet(Router.RepresentativeRouting.NearestRepresentative)]
        public async Task<IActionResult> GetNearestRepresentative(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude)
        {
            var query = new GetNearestRepresentativeQuery(fromLatitude, fromLongitude, toLatitude, toLongitude);
            var result = await Mediator.Send(query);
            return NewResult(result);
        }

        [HttpGet(Router.RepresentativeRouting.UpdateRepresentativeLivelocation)]
        public async Task<IActionResult> UpdateRepresentativelocation([FromBody] UpdateRepresentativesLocationCommand command)
        {

            var result = await Mediator.Send(command);
            return NewResult(result);
        }
        [HttpPut(Router.RepresentativeRouting.UpdateRepresentativeInfo)]
        public async Task<IActionResult> UpdateRepresentativeInfo([FromQuery] UpdateRepresentativeInfoCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
        [HttpPost(Router.RepresentativeRouting.DriverArrived)]
        public async Task<IActionResult> DriverArrived([FromRoute] int orderId)
        {
            var result = await _representativeService.MarkDriverArrivedAsync(orderId);
            return Ok(result);
        }
        [HttpPut(Router.RepresentativeRouting.MarkDriverArrived)]
        public async Task<IActionResult> MarkClientLate([FromRoute] int orderId)
        {
            var command = new MarkIsClientLateCommand(orderId);
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
    }
}