using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.EF.Configurations
{
    public class PruchaseItemConfig : IEntityTypeConfiguration<PurchaseItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseItem> builder)
        {
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
        }
    }
}
