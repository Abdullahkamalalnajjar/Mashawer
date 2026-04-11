using Mashawer.Api.Base;
using Mashawer.Core.Features.Wallets.Commands.Models;
using Mashawer.Core.Features.Wallets.Queries.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{
    //  [Authorize]
    public class WalletController : AppBaseController
    {
        [HttpGet("get-wallet/{userId}")]
        public async Task<IActionResult> GetWalletByUserId(string userId)
        {
            var query = new GetWalletByUserIdQuery { UserId = userId };
            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        [HttpGet("get-balance/{userId}")]
        public async Task<IActionResult> GetBalanceByUserId(string userId)
        {
            var query = new GetBalanceByUserIdQuery { UserId = userId };
            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        [HttpPut("update-balance")]
        public async Task<IActionResult> UpdateBalance(UpdateWalletBalanceCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut("update-disable-status")]
        public async Task<IActionResult> UpdateDisableStatus(UpdateWalletDisableStatusCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
    }
}
