namespace Mashawer.EF.Repositories
{
    public class MessageRepository : GenericRepository<ChatMessage>, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
