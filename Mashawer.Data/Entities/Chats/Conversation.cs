namespace Mashawer.Data.Entities
{
    public class Conversation
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<ChatMessage> Messages { get; set; }


        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Receiver { get; set; }
    }
}
