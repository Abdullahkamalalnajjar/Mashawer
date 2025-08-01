using Mashawer.Api.Base;
using Mashawer.Core.Features.Representatives.Queries.Models;
using Mashawer.Data.AppMetaData;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{
    public class RepresentativeController : AppBaseController
    {
        [HttpGet(Router.RepresentativeRouting.GetApprovedRepresentativesByAddress)]
        public async Task<IActionResult> GetRepresentativesByAddress(string address)
        {
            var query = new GetAllRepresentativeByAddress(address);
            var result = await Mediator.Send(query);
            return NewResult(result);
        }
    }
}
