using AutoMapper;
using Mashawer.Data.Dtos;
using Mashawer.Data.Entities;

namespace Mashawer.Core.Mapping.Wallets
{
    public class WalletProfile : Profile
    {
        public WalletProfile()
        {
            CreateMap<Wallet, WalletDto>()
                .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.Transactions));

            CreateMap<WalletTransaction, WalletTransactionDto>();
        }
    }
}
