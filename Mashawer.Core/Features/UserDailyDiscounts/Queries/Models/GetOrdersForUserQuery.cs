namespace Mashawer.Core.Features.UserDailyDiscounts.Queries.Models
{
    public class GetOrdersForUserQuery : IRequest<Response<IEnumerable<UserDailyDiscountDto>>>
    {
        public string UserId { get; set; }
        public GetOrdersForUserQuery(string userId)
        {
            UserId = userId;
        }
    }
}
