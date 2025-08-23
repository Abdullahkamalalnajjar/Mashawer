namespace Mashawer.EF.Repositories
{
    public class WalletTransactionRepository : GenericRepository<WalletTransaction>, IWalletTransactionRepository
    {
        public WalletTransactionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }


}
