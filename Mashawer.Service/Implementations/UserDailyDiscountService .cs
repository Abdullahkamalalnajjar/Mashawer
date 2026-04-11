namespace Mashawer.Service.Implementations
{
    public class UserDailyDiscountService : IUserDailyDiscountService
    {
        private readonly IUnitOfWork _unit;

        public UserDailyDiscountService(IUnitOfWork unit)
        {
            _unit = unit;
        }

        public async Task<string> AddDiscontAsyncForAllNormalUser(decimal discount, DateTime discountDate)
        {
            var normalUsersIds = await _unit.Users.GetTableNoTracking().Where(x => x.UserType == UserType.NormalUser).Select(u => u.Id).ToListAsync();
            foreach (var userId in normalUsersIds)
            {
                //var existingDiscount = await _unit.UserDailyDiscounts.GetTableNoTracking()
                //    .FirstOrDefaultAsync(d => d.UserId == userId && d.DiscountDate == DateTime.UtcNow.Date);

                var userDiscount = new UserDailyDiscount
                {
                    UserId = userId,
                    DiscountDate = discountDate,
                    DiscountAmount = discount,
                    IsUsed = false
                };
                await _unit.UserDailyDiscounts.AddAsync(userDiscount);

            }
            await _unit.CompeleteAsync();
            return "Added";

        }

        public async Task AddDiscountAsync(string userId, DateTime date, decimal amount)
        {
            var discount = new UserDailyDiscount
            {
                UserId = userId,
                DiscountDate = date.Date,
                DiscountAmount = amount,
                IsUsed = false
            };

            await _unit.UserDailyDiscounts.AddAsync(discount);
        }

        public async Task<List<UserDailyDiscountDto>> GetAllUserDiscountAsync()
        {
            return await _unit.UserDailyDiscounts
              .GetTableNoTracking().Select(x => new UserDailyDiscountDto
              {
                  Id = x.Id,
                  UserId = x.UserId,
                  UserName = x.User.FullName,
                  DiscountDate = x.DiscountDate,
                  DiscountAmount = x.DiscountAmount,
                  IsUsed = x.IsUsed
              }).ToListAsync();
        }

        public async Task<IEnumerable<UserDailyDiscountDto>> GetUserDiscountAsync(string userId)
        {
            return await _unit.UserDailyDiscounts.GetTableNoTracking().Where(x => x.UserId == userId && !x.IsUsed)
                .Select(x => new UserDailyDiscountDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    UserName = x.User.FullName,
                    DiscountDate = x.DiscountDate,
                    DiscountAmount = x.DiscountAmount,
                    IsUsed = x.IsUsed
                }).ToListAsync();
        }

        public async Task<string> MarkUsedAsync(List<int> ids)
        {
            foreach (var id in ids)
            {
                var discount = await _unit.UserDailyDiscounts.GetByIdAsync(id);

                if (discount == null)
                    continue;  // تجاهل لو مش موجود

                discount.IsUsed = true;
                _unit.UserDailyDiscounts.Update(discount);
            }

            await _unit.CompeleteAsync();
            return "MarkedAsUsed";
        }

    }

}
