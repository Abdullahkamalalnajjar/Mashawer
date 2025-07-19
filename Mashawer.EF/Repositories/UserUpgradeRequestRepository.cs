namespace Mashawer.EF.Repositories
{
    public class UserUpgradeRequestRepository : GenericRepository<UserUpgradeRequest>, IUserUpgradeRequestRepository
    {
        public UserUpgradeRequestRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
