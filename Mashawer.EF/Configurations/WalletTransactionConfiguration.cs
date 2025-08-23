
namespace Mashawer.EF.Configurations
{
    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");
        }
    }
}
