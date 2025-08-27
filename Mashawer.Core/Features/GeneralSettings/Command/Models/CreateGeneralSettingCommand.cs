namespace Mashawer.Core.Features.GeneralSettings.Command.Models
{
    public class CreateGeneralSettingCommand : IRequest<Response<string>>
    {
        public decimal DiscountPercentage { get; set; }

    }
}
