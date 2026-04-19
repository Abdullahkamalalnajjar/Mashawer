using Mashawer.Data.Entities;
using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Interfaces;

namespace Mashawer.Core.Helpers
{
    internal static class OrderCancellationWalletHelper
    {
        internal const decimal CancellationFeeAmount = 10m;
        internal static readonly TimeSpan CancellationFeeDelay = TimeSpan.FromMinutes(7);

        internal static bool ShouldApplyCancellationFee(Order order)
        {
            return DateTime.UtcNow - order.CreatedAt >= CancellationFeeDelay;
        }

        internal static async Task AdjustWalletBalanceAsync(
            IUnitOfWork unitOfWork,
            string userId,
            decimal delta,
            string transactionType,
            int orderId,
            CancellationToken cancellationToken)
        {
            if (delta == 0)
                return;

            var wallet = await unitOfWork.Wallets
                .GetTableAsTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

            if (wallet == null)
            {
                wallet = new Wallet
                {
                    UserId = userId,
                    Balance = 0
                };

                await unitOfWork.Wallets.AddAsync(wallet, cancellationToken);
            }

            wallet.Balance += delta;

            await unitOfWork.WalletTransactions.AddAsync(new WalletTransaction
            {
                Wallet = wallet,
                Amount = Math.Abs(delta),
                Type = transactionType,
                Status = "Paid",
                PaidAt = DateTime.UtcNow,
                OrderId = orderId
            }, cancellationToken);
        }
    }
}
