namespace Mashawer.Service.Abstracts
{
    public interface IWalletService
    {
        Task<string> UpdateWalletBalanceAsync(int walletId, decimal amount, string type, CancellationToken cancellationToken);
        Task<string> UpdateWalletBalanceAsync(string userId, decimal amount, string type, CancellationToken cancellationToken);
        Task<string> UpdateWalletDisableStatusAsync(int walletId, bool isDisable, CancellationToken cancellationToken);
        Task<Wallet?> GetWalletByIdAsync(int walletId);
        Task<Wallet?> GetWalletByUserIdAsync(string userId);
        Task<decimal> GetBalanceByUserIdAsync(string userId);
    }
}
