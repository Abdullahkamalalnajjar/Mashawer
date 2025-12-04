namespace Mashawer.EF.Configurations
{
    public class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            // Primary Key
            builder.HasKey(r => r.Id);

            // Relationship with RatedBy (AspNetUsers)
            builder.HasOne(r => r.RatedBy)
                   .WithMany()
                   .HasForeignKey(r => r.RatedById)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Relationship with RatedUser (AspNetUsers)
            builder.HasOne(r => r.RatedUser)
                   .WithMany()
                   .HasForeignKey(r => r.RatedUserId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Additional Configuration
            builder.Property(r => r.Stars)
                   .IsRequired();

            builder.Property(r => r.Comment)
                   .HasMaxLength(1000); // Optional: Limit the comment length
        }
    }
}