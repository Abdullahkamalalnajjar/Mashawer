﻿
namespace Mashawer.EF
{
    public static class ModuelEFDependancies
    {
        public static IServiceCollection AddEFDependencies(this IServiceCollection services)
        {
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserUpgradeRequestRepository, UserUpgradeRequestRepository>();
            services.AddTransient<IDeleteRecoredRepository, DeleteRecoredRepository>();
            services.AddTransient<IWalletRepository, WalletRepository>();
            services.AddTransient<IWalletTransactionRepository, WalletTransactionRepository>();



            return services;
        }
    }
}
