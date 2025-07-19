using Mashawer.Api.Base;
using Mashawer.Core.Features.Claims.Queries.Models;
using Mashawer.Data.Consts;
using Mashawer.Data.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{

    public class ClaimController : AppBaseController
    {
        [HasPermission(Permissions.GetClaims)]
        [HttpGet("GetClaims")]
        public async Task<IActionResult> GetClaims()
        {
            var query = new GetPermissionsQuery();
            var response = await Mediator.Send(query);
            return Ok(response);
        }
        [HasPermission(Permissions.GetClaims)]
        [HttpGet("GetClaims/{id}")]
        public async Task<IActionResult> GetClaims(string id)
        {
            var query = new GetPermissionsByRoleIdQuery { RoleId = id };
            var response = await Mediator.Send(query);
            return Ok(response);
        }

    }
}
