

using Microsoft.AspNetCore.Authorization;

namespace Mashawer.EF
{
    public static class ServiceRegisteration
    {
        public static IServiceCollection AddServiceRegisteration(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddHttpContextAccessor();

            services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
            {
                // Password settings.
                option.Password.RequireDigit = false;
                option.Password.RequireLowercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
                option.Password.RequiredLength = 6;
                option.Password.RequiredUniqueChars = 1;


                // Lockout settings.
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                option.Lockout.MaxFailedAccessAttempts = 5;
                option.Lockout.AllowedForNewUsers = true;

                // User settings.
                option.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                option.User.RequireUniqueEmail = true;
                option.SignIn.RequireConfirmedEmail = true;

            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            //JWT AUTHENTICATE
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings))); // for dependency injection
            var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>(); // for direct access
            // mail settings
            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings))); // for dependency injection
            services.Configure<StripeSettings>(configuration.GetSection(nameof(StripeSettings))); // for dependency injection
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtSettings.ValidateIssuer,
                    ValidIssuers = new[] { jwtSettings.Issuer },
                    ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                    ValidAudience = jwtSettings.Audience,
                    ValidateAudience = jwtSettings.ValidateAudience,
                    ValidateLifetime = jwtSettings.ValidateLifeTime,
                };

                // ✅ إضافة رسالة خطأ مخصصة عند رفض المصادقة
                x.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse(); // منع الرد الافتراضي
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var result = JsonConvert.SerializeObject(new
                        {
                            statusCode = 401,
                            message = "غير مصرح لك، يُرجى تسجيل الدخول باستخدام التوكن."
                        });

                        return context.Response.WriteAsync(result);
                    }
                };
            });


            services.AddAuthorization(options =>
            {

            });
            return services;
        }
    }
}
