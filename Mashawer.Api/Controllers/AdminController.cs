using Mashawer.Api.Base;
using Mashawer.Core.Features.Admin.Command.Models;
using Mashawer.Core.Features.Admin.Query.Model;
using Mashawer.Data.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{

    public class AdminController : AppBaseController
    {
        [HttpPost("accept-or-reject-request")]
        public async Task<IActionResult> AcceptOrRejectRequestAsync([FromBody] AcceptOrRejectRequestCommand command, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(command, cancellationToken);
            return NewResult(response);
        }
        [HttpGet("orders-by-status-or-address")]
        public async Task<IActionResult> GetOrdersByStatusOrAddressAsync([FromQuery] OrderStatus orderStatus, string? address, DateTime? dateTime)
        {
            var query = new GetOrdersByStatusAndAddressQuery(orderStatus, address, dateTime);
            var response = await Mediator.Send(query);
            return NewResult(response);
        }
    }
}
