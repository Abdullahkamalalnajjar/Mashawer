using Mashawer.Api.Base;
using Mashawer.Core.Features.ClientCancelOrders.Command.Model;
using Mashawer.Data.AppMetaData;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{
    public class ClientCancelOrderController : AppBaseController
    {
        [HttpPost(Router.ClientCancelOrderRouting.Create)]
        public async Task<IActionResult> CreateClientCancelOrder([FromBody] AddClientCancelOrderCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
    }
}