using Mashawer.Core.Features.Wallets.Commands.Models;

namespace Mashawer.Core.Features.Wallets.Commands.Handlers
{
    public class WalletCommandHandler : ResponseHandler,
        IRequestHandler<UpdateWalletBalanceCommand, Response<string>>,
        IRequestHandler<UpdateWalletDisableStatusCommand, Response<string>>
    {
        private readonly IWalletService _walletService;
        private readonly ILogger<WalletCommandHandler> _logger;

        public WalletCommandHandler(IWalletService walletService, ILogger<WalletCommandHandler> logger)
        {
            _walletService = walletService;
            _logger = logger;
        }

        public async Task<Response<string>> Handle(UpdateWalletBalanceCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                return BadRequest<string>("Amount must be greater than zero");

            if (!request.Type.Equals("Deposit", StringComparison.OrdinalIgnoreCase) &&
                !request.Type.Equals("Withdraw", StringComparison.OrdinalIgnoreCase))
                return BadRequest<string>("Type must be either 'Deposit' or 'Withdraw'");

            string result;
            if (!string.IsNullOrWhiteSpace(request.UserId))
            {
                result = await _walletService.UpdateWalletBalanceAsync(request.UserId, request.Amount, request.Type, cancellationToken);
            }
            else if (request.WalletId > 0)
            {
                result = await _walletService.UpdateWalletBalanceAsync(request.WalletId, request.Amount, request.Type, cancellationToken);
            }
            else
            {
                return BadRequest<string>("WalletId or UserId is required");
            }

            return result switch
            {
                "WalletNotFound" => NotFound<string>("Wallet not found"),
                "WalletDisabled" => BadRequest<string>("Wallet is disabled and cannot be updated"),
                "InsufficientBalance" => BadRequest<string>("Insufficient balance for withdrawal"),
                "InvalidTransactionType" => BadRequest<string>("Invalid transaction type"),
                "Updated" => Success<string>("Wallet balance updated successfully"),
                _ => BadRequest<string>("Error updating wallet balance")
            };
        }

        public async Task<Response<string>> Handle(UpdateWalletDisableStatusCommand request, CancellationToken cancellationToken)
        {
            var result = await _walletService.UpdateWalletDisableStatusAsync(request.WalletId, request.IsDisable, cancellationToken);

            var statusText = request.IsDisable ? "disabled" : "enabled";
            return result switch
            {
                "WalletNotFound" => NotFound<string>("Wallet not found"),
                "Updated" => Success<string>($"Wallet {statusText} successfully"),
                _ => BadRequest<string>("Error updating wallet status")
            };
        }
    }
}
