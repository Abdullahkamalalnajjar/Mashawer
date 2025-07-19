using Mashawer.Api.Base;
using Mashawer.Core.Features.Users.Commands.Models;
using Mashawer.Core.Features.Users.Queries.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{

    public class UserController : AppBaseController
    {
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        //   [HasPermission(Permissions.GetUsers)]
        [HttpGet("AllUser")]
        public async Task<IActionResult> AllUsers()
        {
            var request = new GetAllUserQuery();
            var response = await Mediator.Send(request);
            return NewResult(response);
        }
        [HttpGet("AllAgent")]
        public async Task<IActionResult> AllAgents()
        {
            var request = new GetAllAgentQuery();
            var response = await Mediator.Send(request);
            return NewResult(response);
        }
        [HttpGet("AllRepresentative")]
        public async Task<IActionResult> AllRepresentatives()
        {
            var request = new GetAllRepresentativeQuery();
            var response = await Mediator.Send(request);
            return NewResult(response);
        }
        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var request = new GetUserByIdQuery { UserId = userId };
            var response = await Mediator.Send(request);
            return NewResult(response);
        }
    }
}
