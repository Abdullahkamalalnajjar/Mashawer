using Mashawer.Api.Base;
using Mashawer.Core.Features.Otp.Queries.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{
    public class OtpController : AppBaseController
    {
        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyOTpCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
        [HttpPost("resend")]
        public async Task<IActionResult> Resend([FromBody] ResendOtpCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
    }
}