using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;

namespace Mashawer.EF.Configurations
{
    public class OrderTaskConfiguration : IEntityTypeConfiguration<OrderTask>
    {
        public void Configure(EntityTypeBuilder<OrderTask> builder)
        {
            // 🔹 المفتاح الأساسي
            builder.HasKey(t => t.Id);

            // 🔹 العلاقة مع الطلب الرئيسي
            builder.HasOne(t => t.Order)
                   .WithMany(o => o.Tasks)
                   .HasForeignKey(t => t.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 🔹 العنوانين
            builder.OwnsOne(x => x.PickupLocation, n => { n.WithOwner(); });
            builder.OwnsOne(x => x.DeliveryLocation, n => { n.WithOwner(); });

            // 🔹 التحويل للـ Enum
            builder.Property(t => t.Type)
                   .HasConversion(
                       o => o.ToString(),
                       o => (OrderType)Enum.Parse(typeof(OrderType), o))
                   .IsRequired();

            builder.Property(t => t.Status)
                   .HasConversion(
                       o => o.ToString(),
                       o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o))
                   .IsRequired();

            // 🔹 خصائص رقمية
            builder.Property(t => t.DeliveryPrice).HasColumnType("decimal(18,2)");
            builder.Property(t => t.DistanceKm).HasColumnType("decimal(10,2)");

            // ✅ العلاقة مع عناصر المشتريات (تم تصحيحها)
            builder.HasMany(t => t.PurchaseItems)
                   .WithOne(p => p.OrderTask)
                   .HasForeignKey(p => p.OrderTaskId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("OrderTasks");
        }
    }
}
