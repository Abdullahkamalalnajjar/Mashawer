namespace Mashawer.EF.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Content)
                   .HasMaxLength(4000);

            builder.Property(m => m.SentAt)
                   .IsRequired();

            builder.Property(m => m.IsRead)
                   .IsRequired();

            builder.HasOne(m => m.Sender)
                   .WithMany()
                   .HasForeignKey(m => m.SenderId)
                   .OnDelete(DeleteBehavior.Restrict); // ❌ منع الحذف التلقائي هنا

            builder.HasOne(m => m.Receiver)
                   .WithMany()
                   .HasForeignKey(m => m.ReceiverId)
                   .OnDelete(DeleteBehavior.Restrict); // ❌ منع الحذف التلقائي هنا

            builder.HasOne(m => m.Conversation)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(m => m.ConversationId)
                   .OnDelete(DeleteBehavior.Cascade); // ✅ فقط هذا اللي نسمح له بالحذف التلقائي
        }
    }
}
