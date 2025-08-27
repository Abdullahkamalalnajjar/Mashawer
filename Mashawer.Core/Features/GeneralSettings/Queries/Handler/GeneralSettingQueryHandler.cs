using Mashawer.Core.Features.GeneralSettings.Queries.Model;

namespace Mashawer.Core.Features.GeneralSettings.Queries.Handler
{
    public class GeneralSettingQueryHandler(IGeneralSettingService generalSettingService) : ResponseHandler,
        IRequestHandler<GetGeneralSettingQuery, Response<GeneralSetting>>
    {
        private readonly IGeneralSettingService generalSettingService = generalSettingService;

        public async Task<Response<GeneralSetting>> Handle(GetGeneralSettingQuery request, CancellationToken cancellationToken)
        {
            var generalSetting = await generalSettingService.GetGeneralSettingsAsync();
            if (generalSetting == null)
                return NotFound<GeneralSetting>("General setting not found");
            return Success(generalSetting);
        }
    }
}
