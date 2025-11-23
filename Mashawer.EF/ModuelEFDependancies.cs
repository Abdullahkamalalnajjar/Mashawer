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
            services.AddTransient<IGeneralSettingRepository, GeneralSettingRepository>();
            services.AddTransient<IUserNotificationRepository, UserNotificationRepository>();
            services.AddTransient<IPurchaseItemRepository, PurchaseItemRepository>();
            services.AddTransient<IOrderTaskRepository, OrderTaskRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<IClientCancelOrderRepository, ClientCancelOrderRepository>();
            services.AddTransient<IRepresentitiveCancelOrderRepository, RepresentitiveCancelOrderRepository>();

            return services;
        }
    }
}
