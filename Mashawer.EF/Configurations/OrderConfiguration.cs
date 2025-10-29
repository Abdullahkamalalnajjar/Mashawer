using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;

namespace Mashawer.EF.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(x => x.DeliveryLocation, n => { n.WithOwner(); });
            builder.OwnsOne(x => x.PickupLocation, n => { n.WithOwner(); });

            builder.Property(s => s.Status).HasConversion(o => o.ToString(), o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o));
            // builder.Property(s => s.CancelReason).HasConversion(o => o.ToString(), o => (CancelReason)Enum.Parse(typeof(CancelReason), o));

            // builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            //  builder.Property(p => p.PriceAfterDeducation).HasColumnType("decimal(18,2)");
            builder.Property(p => p.DeliveryPrice)
                     .HasColumnType("decimal(18,2)");

            builder.Property(p => p.ItemsTotalCost)
                   .HasColumnType("decimal(18,2)");

            // إعدادات إضافية
            builder.Property(o => o.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
