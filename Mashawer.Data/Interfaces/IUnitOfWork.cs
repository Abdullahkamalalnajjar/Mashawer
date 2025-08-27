
namespace Mashawer.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        IUserRepository Users { get; }
        IDeleteRecoredRepository DeleteRecoreds { get; }
        IUserUpgradeRequestRepository UserUpgradeRequests { get; }
        IOrderRepository Orders { get; }
        IWalletTransactionRepository WalletTransactions { get; }
        IWalletRepository Wallets { get; }
        IGeneralSettingRepository GeneralSettings { get; }
        Task<int> CompeleteAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();

    }
}
