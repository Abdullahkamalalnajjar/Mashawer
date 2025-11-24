using Mashawer.Api.Base;
using Mashawer.Core.Features.ClientCancelOrders.Command.Model;
using Mashawer.Core.Features.RepresentitiveCancelOrders.Command.Models;
using Mashawer.Data.AppMetaData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{

    public class RepresentitiveCancelOrderController : AppBaseController
    {
        [HttpPost(Router.RepresentitiveCancelOrderRouting.Create)]
        public async Task<IActionResult> CreateRepresentitiveCancelOrder([FromBody] AddRepresentitiveCancelOrderCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
    }
}
