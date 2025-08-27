namespace Mashawer.EF.Configurations
{
    public class GeneralSettingConfiguration : IEntityTypeConfiguration<GeneralSetting>
    {
        public void Configure(EntityTypeBuilder<GeneralSetting> builder)
        {


            builder.Property(p => p.DiscountPercentage).HasColumnType("decimal(18,2)");
        }
    }
}
