namespace Mashawer.Core.Features.Wallets.Queries.Models
{
    public class GetBalanceByUserIdQuery : IRequest<Response<decimal>>
    {
        public string UserId { get; set; }
    }
}
