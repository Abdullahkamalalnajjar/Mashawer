namespace Mashawer.Data.Entities
{
    public class Wallet
    {
        public int Id { get; set; }
        public string UserId { get; set; }   // ربط مع الـ ApplicationUser
        public ApplicationUser User { get; set; } = null!;
        public decimal Balance { get; set; } = 0;
        public List<WalletTransaction> Transactions { get; set; } = new();
    }
}
