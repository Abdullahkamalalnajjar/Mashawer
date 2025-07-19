using Mashawer.Api.Base;
using Mashawer.Core.Features.GoogleService.Model;
using Mashawer.Data.AppMetaData;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{

    public class GoogleServiceController : AppBaseController
    {
        [HttpGet(Router.GoogleRouting.GoogleSignIn)]
        public async Task<IActionResult> SignIn([FromQuery] GoogleSignInCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
    }
}
