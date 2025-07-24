using Mashawer.Api.Base;
using Mashawer.Core.Features.UserUpgradeRequests.Command.Models;
using Mashawer.Core.Features.UserUpgradeRequests.Queries.Models;
using Mashawer.Data.AppMetaData;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{

    public class UserUpgradeRequestController : AppBaseController
    {
        [HttpPost(Router.UserUpgradeRequestRouting.Create)]
        public async Task<IActionResult> Create([FromForm] CreateUserUpgradeRequestCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
        [HttpPut(Router.UserUpgradeRequestRouting.Edit)]
        public async Task<IActionResult> Edit([FromForm] EditUserUpgradeRequestCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
        [HttpDelete(Router.UserUpgradeRequestRouting.Delete)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await Mediator.Send(new DeleteUserUpgradeRequestCommand(id));
            return NewResult(result);
        }
        [HttpGet(Router.UserUpgradeRequestRouting.GetById)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await Mediator.Send(new GetUserUpgradeRequestByIdQuery(id));
            return NewResult(result);
        }
        [HttpGet(Router.UserUpgradeRequestRouting.List)]
        public async Task<IActionResult> List()
        {
            var result = await Mediator.Send(new GetUserUpgradeRequestListQuery());
            return NewResult(result);
        }
        [HttpGet(Router.UserUpgradeRequestRouting.GetByUserId)]
        public async Task<IActionResult> GetByUserId([FromRoute] string userId)
        {
            var result = await Mediator.Send(new GetUserUpgradeRequestByUserIdQuery(userId));
            return NewResult(result);
        }
        [HttpGet(Router.UserUpgradeRequestRouting.GetByAgentId)]
        public async Task<IActionResult> GetByAgentId([FromRoute] string agentId)
        {
            var result = await Mediator.Send(new GetUserUpgradeRequestByAgentIdQuery(agentId));
            return NewResult(result);
        }
        [HttpGet(Router.UserUpgradeRequestRouting.GetByAddress)] // المحافظة
        public async Task<IActionResult> GetByAddress([FromRoute] string address)
        {
            var result = await Mediator.Send(new GetAllUserUpgradeRequestByAddressQuery(address));
            return NewResult(result);
        }


    }
}
