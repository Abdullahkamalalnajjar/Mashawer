namespace Mashawer.EF.Repositories
{
    public class UserNotificationRepository : GenericRepository<UserNotification>, IUserNotificationRepository
    {
        public UserNotificationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
