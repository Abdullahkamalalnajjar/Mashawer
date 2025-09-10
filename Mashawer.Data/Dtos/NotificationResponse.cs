namespace Mashawer.Data.Dtos
{
    public class NotificationResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }

    }
}
