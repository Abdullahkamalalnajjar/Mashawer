using Microsoft.Extensions.Logging;

namespace Mashawer.Service.Implementations
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WalletService> _logger;

        public WalletService(IUnitOfWork unitOfWork, ILogger<WalletService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<string> UpdateWalletBalanceAsync(int walletId, decimal amount, string type, CancellationToken cancellationToken)
        {
            try
            {
                var wallet = await _unitOfWork.Wallets.GetTableAsTracking().FirstOrDefaultAsync(w => w.Id == walletId, cancellationToken);

                if (wallet == null)
                    return "WalletNotFound";

                if (wallet.IsDisable)
                    return "WalletDisabled";

                // Update balance based on transaction type
                var lowerCaseType = type.ToLower();
                if (lowerCaseType == "deposit" || lowerCaseType == "refund")
                {
                    wallet.Balance += amount;
                }
                else if (lowerCaseType == "withdraw" || lowerCaseType == "orderpayment" || lowerCaseType == "orderfee")
                {
                    if (wallet.Balance < amount)
                        return "InsufficientBalance";
                    wallet.Balance -= amount;
                }
                else
                {
                    _logger.LogWarning($"Invalid transaction type for wallet update: {type}");
                    return "InvalidTransactionType";
                }

                // Update the wallet
                _unitOfWork.Wallets.Update(wallet);
                await _unitOfWork.CompeleteAsync();

                // Create transaction record
                var transaction = new WalletTransaction
                {
                    WalletId = walletId,
                    Amount = amount,
                    Type = type,
                    Status = "Paid",
                    PaidAt = DateTime.UtcNow
                };

                await _unitOfWork.WalletTransactions.AddAsync(transaction, cancellationToken);
                await _unitOfWork.CompeleteAsync();

                _logger.LogInformation($"Wallet {walletId} balance updated. Type: {type}, Amount: {amount}");
                return "Updated";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating wallet balance for WalletId: {walletId}");
                return "Error";
            }
        }

        public async Task<string> UpdateWalletDisableStatusAsync(int walletId, bool isDisable, CancellationToken cancellationToken)
        {
            try
            {
                var wallet = await _unitOfWork.Wallets.GetTableNoTracking()
                    .FirstOrDefaultAsync(w => w.Id == walletId, cancellationToken);

                if (wallet == null)
                    return "WalletNotFound";

                wallet.IsDisable = isDisable;
                _unitOfWork.Wallets.Update(wallet);
                await _unitOfWork.CompeleteAsync();

                _logger.LogInformation($"Wallet {walletId} disable status updated to: {isDisable}");
                return "Updated";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating wallet disable status for WalletId: {walletId}");
                return "Error";
            }
        }

        public async Task<Wallet?> GetWalletByIdAsync(int walletId)
        {
            return await _unitOfWork.Wallets.GetTableNoTracking()
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.Id == walletId);
        }

        public async Task<Wallet?> GetWalletByUserIdAsync(string userId)
        {
            return await _unitOfWork.Wallets.GetTableNoTracking()
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<decimal> GetBalanceByUserIdAsync(string userId)
        {
            return await _unitOfWork.Wallets.GetTableNoTracking()
                .Where(w => w.UserId == userId)
                .Select(w => w.Balance)
                .FirstOrDefaultAsync();
        }
    }
}
