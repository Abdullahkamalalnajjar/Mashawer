namespace Mashawer.Data.Dtos.Chat
{
    public class SendMessageDTO
    {
        public int ConversationId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }

    }
}
