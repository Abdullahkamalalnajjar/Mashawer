namespace Mashawer.Core.Features.Wallets.Commands.Models
{
    public class UpdateWalletDisableStatusCommand : IRequest<Response<string>>
    {
        public int WalletId { get; set; }
        public bool IsDisable { get; set; }
    }
}
