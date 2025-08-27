namespace Mashawer.Core.Features.GeneralSettings.Command.Models
{
    public class EditGeneralSettingCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
        public decimal DiscountPercentage { get; set; }

    }
}
