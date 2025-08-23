namespace Mashawer.EF.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.Property(p => p.Balance).HasColumnType("decimal(18,2)");
        }
    }


}
