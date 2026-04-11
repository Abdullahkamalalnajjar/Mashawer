using Mashawer.Api.Base;
using Mashawer.Core.Features.UserDailyDiscounts.Command.Models;
using Mashawer.Core.Features.UserDailyDiscounts.Queries.Models;
using Mashawer.Data.AppMetaData;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{

    public class UserDailyDiscountController : AppBaseController
    {
        [HttpPost(Router.UserDailyDiscountRouting.Create)]
        public async Task<IActionResult> AddDailyDiscount([FromQuery] AddUserDailyDiscountCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
        [HttpGet(Router.UserDailyDiscountRouting.GetByUserIdAndDate)]
        public async Task<IActionResult> GetDailyDiscountByUserIdAndDate([FromRoute] string userId)
        {
            var query = new GetOrdersForUserQuery(userId);
            var result = await Mediator.Send(query);
            return NewResult(result);
        }
        [HttpGet(Router.UserDailyDiscountRouting.List)]
        public async Task<IActionResult> GetAllUserDiscount()
        {
            var userDailyDiscount = new GetAllUserDiscountQuery();
            var result = await Mediator.Send(userDailyDiscount);
            return NewResult(result);
        }

        [HttpPut(Router.UserDailyDiscountRouting.MarkAsUsed)]
        public async Task<IActionResult> MarkAsUsed([FromQuery] List<int> id)
        {
            var command = new MarkUsedCommand(id);
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
        [HttpPost(Router.UserDailyDiscountRouting.AddForAllNormalUsers)]
        public async Task<IActionResult> AddDiscountForAllNormalUsers([FromQuery] AddDicscontForAllNormalUserCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
    }
}
