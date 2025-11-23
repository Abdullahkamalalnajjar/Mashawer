using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.EF.Repositories
{
    public class ClientCancelOrderRepository : GenericRepository<ClientCancelOrder>, IClientCancelOrderRepository
    {
        public ClientCancelOrderRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}