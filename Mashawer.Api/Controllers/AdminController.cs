using Mashawer.Api.Base;
using Mashawer.Core.Features.Admin.Command.Models;
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
    }
}
