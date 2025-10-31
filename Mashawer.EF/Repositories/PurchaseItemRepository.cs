using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.EF.Repositories
{
    public class PurchaseItemRepository : GenericRepository<PurchaseItem>, IPurchaseItemRepository
    {
        public PurchaseItemRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
