using Microsoft.Extensions.DependencyInjection;

namespace Mashawer.Service
{
    public static class ModuelServiceDependancies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            services.AddTransient<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IJwtProvider, JwtProvider>();
            services.AddTransient<IEmailSender, EmailService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IClaimService, ClaimService>();
            services.AddTransient<IGoogleService, GoogleService>();
            services.AddTransient<IUserUpgradeRequestService, UserUpgradeRequestService>();
            services.AddTransient<IOtpService, OtpService>();
            services.AddTransient<IAdminService, AdminService>();
            services.AddTransient<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IRepresentativeService, RepresentativeService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IGeneralSettingService, GeneralSettingService>();
            services.AddHttpClient<PaymobService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(60);
            });
            services.AddScoped<PaymobService>();

            return services;
        }
    }
}
