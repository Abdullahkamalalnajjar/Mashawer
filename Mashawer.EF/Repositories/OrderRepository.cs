using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.EF.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }
    }

}

