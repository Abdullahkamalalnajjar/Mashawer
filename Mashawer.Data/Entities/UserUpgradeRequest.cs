using Mashawer.Data.Enums;

namespace Mashawer.Data.Entities
{
    public class UserUpgradeRequest
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public RequestedRole RequestedRole { get; set; } //enum لتحديد الدور المطلوب
        public string? Note { get; set; }
        public string Address { get; set; }
        public UpgradeRequestStatus Status { get; set; } = UpgradeRequestStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // للمندوب فقط
        public string? TargetAgentId { get; set; }
        public ApplicationUser? TargetAgent { get; set; }


    }
}
