namespace Mashawer.EF.Configurations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CreatedAt)
                   .IsRequired();

            builder.HasOne(c => c.Sender)
                   .WithMany()
                   .HasForeignKey(c => c.SenderId)
                   .OnDelete(DeleteBehavior.Restrict); // ✅ لتجنب multiple cascade paths

            builder.HasOne(c => c.Receiver)
                   .WithMany()
                   .HasForeignKey(c => c.ReceiverId)
                   .OnDelete(DeleteBehavior.Cascade); // أو .Restrict لو تحب تمنع حذف المحادثات نهائياً
        }
    }
}
