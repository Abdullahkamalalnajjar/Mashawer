namespace Mashawer.Data.Entities
{
    public class UserNotification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        //public int? OrderId { get; set; }
        //public Order? Order { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
