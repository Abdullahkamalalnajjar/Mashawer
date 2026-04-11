namespace Mashawer.Data.Entities
{
    public class UserDailyDiscount
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime DiscountDate { get; set; }

        public decimal DiscountAmount { get; set; }

        public bool IsUsed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
