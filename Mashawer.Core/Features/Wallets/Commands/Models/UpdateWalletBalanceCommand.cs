namespace Mashawer.Core.Features.Wallets.Commands.Models
{
    public class UpdateWalletBalanceCommand : IRequest<Response<string>>
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // "Deposit" or "Withdraw"
    }
}
