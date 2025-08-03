using Mashawer.Api.Base;
using Mashawer.Core.Features.Orders.Commands.Models;
using Mashawer.Core.Features.Orders.Queries.Models;
using Mashawer.Data.AppMetaData;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{

    public class OrderController : AppBaseController
    {
        [HttpPost(Router.OrderRouting.Create)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
        [HttpGet(Router.OrderRouting.List)]
        public async Task<IActionResult> GetOrders()
        {
            var result = await Mediator.Send(new GetOrdersQuery());
            return NewResult(result);
        }
    }
}
