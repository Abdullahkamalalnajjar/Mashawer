using Mashawer.Api.Base;
using Mashawer.Core.Features.Authentication.Command.Models;
using Mashawer.Core.Features.Users.Commands.Models;
using Mashawer.Core.Features.Users.Queries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{
    [Authorize]
    public class AccountController : AppBaseController
    {
        [AllowAnonymous]
        [HttpGet("UserProfile")]
        public async Task<IActionResult> Info(string userId)
        {
            var request = new UserProfileQuery { UserId = userId };
            var response = await Mediator.Send(request);
            return NewResult(response);
        }

        [HttpPut("add-profileImage")]
        public async Task<IActionResult> AddProfileImage(AddProfileImageCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPut("EditUserProfile")]
        public async Task<IActionResult> UpdateInfo(EditApplicationUserCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpDelete("deleteUser-with-reason")]
        public async Task<IActionResult> DeleteUserWithReason(DeleteUserWithReasonCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUse(DeleteUserCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
    }
}
