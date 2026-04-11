using Mashawer.Data.Dtos;

namespace Mashawer.Core.Features.Wallets.Queries.Models
{
    public class GetWalletByUserIdQuery : IRequest<Response<WalletDto>>
    {
        public string UserId { get; set; }
    }
}