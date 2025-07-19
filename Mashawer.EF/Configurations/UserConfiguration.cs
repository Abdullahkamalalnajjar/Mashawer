using Mashawer.Data.Enums;

namespace Mashawer.EF.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {

            //Default Data
            var admin = new ApplicationUser
            {
                Id = DefaultUsers.AdminId,
                FirstName = "Mashawer",
                LastName = "Admin",
                UserName = DefaultUsers.AdminEmail,
                NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
                Email = DefaultUsers.AdminEmail,
                NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                SecurityStamp = DefaultUsers.AdminSecurityStamp,
                ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
                EmailConfirmed = true,
            };

            // استخدم PasswordHasher لتوليد الـ PasswordHash
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            admin.PasswordHash = "AQAAAAIAAYagAAAAEBDQtLhx3P3q2s2VUfY4MQ4YW8CK+Utz+LJ36vMVUX00IxkwbNR5aVSWIjAIRU+Dgg==";

            builder.HasData(admin);

            builder.Property(s => s.UserType).HasConversion(o => o.ToString(), o => (UserType)Enum.Parse(typeof(UserType), o));



        }
    }
}
