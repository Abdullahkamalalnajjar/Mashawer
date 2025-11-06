namespace Mashawer.Data.Dtos
{
    public class MessageResponse
    {
        public int ConversationId { get; set; }
        public SenderResponse Sender { get; set; } = new();
        public RecipientResponse Recipient { get; set; } = new();
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
    public class SenderResponse
    {
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderImage { get; set; } = "default_image_url"; // يمكن استبدالها بصورة افتراضية
    }
    public class RecipientResponse
    {
        public string RecipientId { get; set; } = string.Empty;
        public string RecipientName { get; set; } = string.Empty;
        public string RecipientImage { get; set; } = "default_image_url"; // يمكن استبدالها بصورة افتراضية
    }
}