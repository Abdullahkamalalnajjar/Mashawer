using Mashawer.Data.Enums;

namespace Mashawer.EF.Configurations
{
    public class UserUpgradeRequestConfiguration : IEntityTypeConfiguration<UserUpgradeRequest>
    {
        public void Configure(EntityTypeBuilder<UserUpgradeRequest> builder)
        {
            builder.Property(s => s.Status).HasConversion(o => o.ToString(), o => (UpgradeRequestStatus)Enum.Parse(typeof(UpgradeRequestStatus), o));

        }
    }
}
