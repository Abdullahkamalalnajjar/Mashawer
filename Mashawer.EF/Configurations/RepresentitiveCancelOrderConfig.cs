using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.EF.Configurations
{
    public class RepresentitiveCancelOrderConfig : IEntityTypeConfiguration<RepresentitiveCancelOrder>
    {
        public void Configure(EntityTypeBuilder<RepresentitiveCancelOrder> builder)
        {

            // Relationship with User (AspNetUsers)
            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // allowed

            // Relationship with Orders
            builder.HasOne(x => x.Order)
                   .WithMany()
                   .HasForeignKey(x => x.OrderId)
                   .OnDelete(DeleteBehavior.Restrict); // prevent multiple cascade paths

        }
    }
}
