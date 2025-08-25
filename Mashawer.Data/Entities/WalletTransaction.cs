namespace Mashawer.Data.Entities
{
    public class WalletTransaction
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending - Paid - Failed
        public string Type { get; set; } = "Deposit";   // Deposit, Withdraw
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public long OrderId { get; set; }
        public string? MerchantOrderId { get; set; }
    }
}
