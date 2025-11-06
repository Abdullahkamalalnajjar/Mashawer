using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.EF
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
        : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add your custom model configurations here  
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }



        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<DeletedRecord> DeletedRecords { get; set; }
        public DbSet<UserUpgradeRequest> UserUpgradeRequests { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<GeneralSetting> GeneralSettings { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<OrderTask> OrderTasks { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> Messages { get; set; }
    }
}
