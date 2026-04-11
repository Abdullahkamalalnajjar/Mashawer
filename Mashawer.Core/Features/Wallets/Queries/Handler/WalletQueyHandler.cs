
using Mashawer.Core.Features.Wallets.Queries.Models;
using AutoMapper;
using Mashawer.Data.Dtos;

namespace Mashawer.Core.Features.Wallets.Queries.Handler
{
    public class WalletQueyHandler(IWalletService walletService, IMapper mapper) : ResponseHandler,
        IRequestHandler<GetWalletByUserIdQuery, Response<WalletDto>>,
        IRequestHandler<GetBalanceByUserIdQuery, Response<decimal>>
    {
        private readonly IWalletService _walletService = walletService;
        private readonly IMapper _mapper = mapper;

        public async Task<Response<WalletDto>> Handle(GetWalletByUserIdQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _walletService.GetWalletByUserIdAsync(request.UserId);
            if (wallet is null)
                return NotFound<WalletDto>(message: "Wallet not found");
            
            var walletDto = _mapper.Map<WalletDto>(wallet);
            return Success(walletDto, "Wallet retrieved successfully");
        }

        public async Task<Response<decimal>> Handle(GetBalanceByUserIdQuery request, CancellationToken cancellationToken)
        {

            var balance = await _walletService.GetBalanceByUserIdAsync(request.UserId);
            return Success(balance, "Balance retrieved successfully");
        }
    }
}
