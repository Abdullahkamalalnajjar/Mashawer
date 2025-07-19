using Mashawer.Api.Base;
using Mashawer.Core.Features.Authentication.Command.Models;
using Mashawer.Data.AppMetaData;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{

    public class AuthenticationController : AppBaseController
    {
        [HttpPost(Router.AuthenticationRouting.SignUp)]
        public async Task<IActionResult> SignUp([FromBody] SignUpUserCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPost(Router.AuthenticationRouting.SginIn)]

        public async Task<IActionResult> SignIn([FromBody] SignInCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPost(Router.AuthenticationRouting.RefreshToken)]
        public async Task<IActionResult> RefreshToken([FromBody] CreateNewRefreshTokenCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPost(Router.AuthenticationRouting.RevokeRefreshToken)]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RevokeRefreashTokenCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpGet(Router.AuthenticationRouting.ConfirmEmail)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPost(Router.AuthenticationRouting.ResendConfirmEmail)]
        public async Task<IActionResult> ResendConfirmEmail(ResendConfirmEmailCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

    }

}
