using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.EF.Repositories
{
    public class RepresentitiveCancelOrderRepository : GenericRepository<RepresentitiveCancelOrder>, IRepresentitiveCancelOrderRepository
    {
        public RepresentitiveCancelOrderRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}