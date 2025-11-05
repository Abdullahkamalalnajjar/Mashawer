using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mashawer.EF.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // 🔹 المفتاح الأساسي
            builder.HasKey(o => o.Id);
            // 🔹 العلاقة مع المهام
            builder.HasMany(o => o.Tasks)
                   .WithOne(t => t.Order)
                   .HasForeignKey(t => t.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 🔹 تحويل الـ Enum إلى String
            builder.Property(s => s.Status)
                   .HasConversion(
                       o => o.ToString(),
                       o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o))
                   .IsRequired();

            builder.Property(s => s.PaymentMethod)
                   .HasConversion(
                       o => o.ToString(),
                       o => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), o))
                   .IsRequired();

            builder.Property(s => s.PaymentStatus)
                   .HasConversion(
                       o => o.ToString(),
                       o => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), o))
                   .IsRequired();

            // 🔹 الدوال الرقمية (الأسعار)
            builder.Property(p => p.TotalDeliveryPrice).HasColumnType("decimal(18,2)");
            builder.Property(p => p.TotalPrice).HasColumnType("decimal(18,2)");
            builder.Property(p => p.DeducationDelivery).HasColumnType("decimal(18,2)");

            // 🔹 التاريخ الافتراضي
            builder.Property(o => o.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.ToTable("Orders");
        }
    }
}
