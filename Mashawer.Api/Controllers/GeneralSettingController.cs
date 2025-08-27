using Mashawer.Api.Base;
using Mashawer.Core.Features.GeneralSettings.Command.Models;
using Mashawer.Core.Features.GeneralSettings.Queries.Model;
using Mashawer.Data.AppMetaData;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{
    public class GeneralSettingController : AppBaseController
    {
        [HttpPost(Router.GeneralSettingRouting.Create)]
        public async Task<IActionResult> CreateGeneralSetting([FromBody] CreateGeneralSettingCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
        [HttpPut(Router.GeneralSettingRouting.Edit)]
        public async Task<IActionResult> UpdateGeneralSetting([FromBody] EditGeneralSettingCommand command)
        {
            var result = await Mediator.Send(command);
            return NewResult(result);
        }
        [HttpGet(Router.GeneralSettingRouting.GetGeneralSetting)]
        public async Task<IActionResult> GetGeneralSetting()
        {
            var result = await Mediator.Send(new GetGeneralSettingQuery());
            return NewResult(result);
        }

    }
}
