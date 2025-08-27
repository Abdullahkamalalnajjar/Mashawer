using Mashawer.Core.Features.GeneralSettings.Command.Models;

namespace Mashawer.Core.Features.GeneralSettings.Command.Handler
{
    public class GeneralSettingCommandHandler(IGeneralSettingService generalSettingService) : ResponseHandler,
        IRequestHandler<CreateGeneralSettingCommand, Response<string>>,
        IRequestHandler<EditGeneralSettingCommand, Response<string>>
    {
        private readonly IGeneralSettingService _generalSettingService = generalSettingService;

        public async Task<Response<string>> Handle(CreateGeneralSettingCommand request, CancellationToken cancellationToken)
        {
            var generalSetting = new GeneralSetting
            {
                DiscountPercentage = request.DiscountPercentage
            };
            var result = await _generalSettingService.CreateGeneralSettingAsync(generalSetting, cancellationToken);
            if (result == "Created")
            {
                return Created("General setting has been created");
            }
            return UnprocessableEntity<string>("Exist error when create general setting");
        }

        public async Task<Response<string>> Handle(EditGeneralSettingCommand request, CancellationToken cancellationToken)
        {
            var generalSetting = await _generalSettingService.GetGeneralSettingsAsync();
            if (generalSetting == null)
            {
                return NotFound<string>("General setting not found");
            }
            generalSetting.DiscountPercentage = request.DiscountPercentage;
            var result = await _generalSettingService.UpdateGeneralSettingAsync(generalSetting);
            if (result == "Updated")
            {
                return Success("General setting has been updated successfully");
            }
            return UnprocessableEntity<string>("Exist error when update general setting");

        }
    }
}
