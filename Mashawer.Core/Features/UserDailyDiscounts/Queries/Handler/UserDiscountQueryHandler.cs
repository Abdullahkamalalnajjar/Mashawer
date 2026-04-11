using Mashawer.Core.Features.UserDailyDiscounts.Queries.Models;

namespace Mashawer.Core.Features.UserDailyDiscounts.Queries.Handler
{
    public class UserDiscountQueryHandler(IUserDailyDiscountService _userDailyDiscountService) : ResponseHandler,
         IRequestHandler<GetAllUserDiscountQuery, Response<List<UserDailyDiscountDto>>>,
        IRequestHandler<GetOrdersForUserQuery, Response<IEnumerable<UserDailyDiscountDto>>>

    {
        private readonly IUserDailyDiscountService userDailyDiscountService = _userDailyDiscountService;

        public async Task<Response<List<UserDailyDiscountDto>>> Handle(GetAllUserDiscountQuery request, CancellationToken cancellationToken)
        {
            var userDailyDiscount = await _userDailyDiscountService.GetAllUserDiscountAsync();

            return Success(userDailyDiscount);

        }

        public async Task<Response<IEnumerable<UserDailyDiscountDto>>> Handle(GetOrdersForUserQuery request, CancellationToken cancellationToken)
        {
            var result = await userDailyDiscountService.GetUserDiscountAsync(request.UserId);
            if (result == null)
            {
                return NotFound<IEnumerable<UserDailyDiscountDto>>("result");
            }
            return Success(result);
        }
    }
}
