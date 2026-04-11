namespace Mashawer.Service.Abstracts
{
    public interface IUserDailyDiscountService
    {
        Task AddDiscountAsync(string userId, DateTime date, decimal amount);
        public Task<string> AddDiscontAsyncForAllNormalUser(decimal discount, DateTime discountDate);
        public Task<IEnumerable<UserDailyDiscountDto>> GetUserDiscountAsync(string userId);
        Task<List<UserDailyDiscountDto>> GetAllUserDiscountAsync();
        public Task<string> MarkUsedAsync(List<int> ids);
    }


}
