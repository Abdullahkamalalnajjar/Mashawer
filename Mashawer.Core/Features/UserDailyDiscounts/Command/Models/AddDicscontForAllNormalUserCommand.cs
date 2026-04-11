namespace Mashawer.Core.Features.UserDailyDiscounts.Command.Models
{
    public class AddDicscontForAllNormalUserCommand : IRequest<Response<string>>
    {

        public DateTime DiscountDate { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}
