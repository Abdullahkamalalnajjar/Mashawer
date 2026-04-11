namespace Mashawer.Data.Dtos
{
    public class WalletDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public bool IsDisable { get; set; }
        public List<WalletTransactionDto> Transactions { get; set; } = new();
    }

    public class WalletTransactionDto
    {
        public int Id { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public long OrderId { get; set; }
        public string? MerchantOrderId { get; set; }
    }
}
