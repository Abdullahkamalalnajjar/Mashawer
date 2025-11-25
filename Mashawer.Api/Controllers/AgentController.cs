using Mashawer.Api.Base;
using Mashawer.Core.Features.Agents.Query.Model;
using Mashawer.Data.AppMetaData;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{

    public class AgentController : AppBaseController
    {
        [HttpGet(Router.AgentRouting.GetOrdersByAgentAddress)]
        public async Task<IActionResult> GetOrdersByAgentAddressAsync(string userId, DateTime? dateTime)
        {
            var query = new GetOrderByAgentAddressQuery(userId, dateTime);
            var result = await Mediator.Send(query);
            return NewResult(result);
        }
    }
}
