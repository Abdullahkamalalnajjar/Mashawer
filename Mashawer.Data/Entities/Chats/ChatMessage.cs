using System.ComponentModel.DataAnnotations.Schema;

namespace Mashawer.Data.Entities
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }
        [ForeignKey("ConversationId")]
        public Conversation Conversation { get; set; }

        public string SenderId { get; set; }
        [ForeignKey("SenderId")]
        public ApplicationUser Sender { get; set; }

        public string ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public ApplicationUser Receiver { get; set; }

        public string? Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
        public string? FileUrl { get; set; }

    }
}
