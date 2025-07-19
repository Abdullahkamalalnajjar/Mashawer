namespace Mashawer.Data.Dtos
{
    public class UserUpgradeRequestResponse
    {
        public int Id { get; set; }

        public string UserId { get; set; } 
        public string UserName { get; set; } 
        public string UserEmail { get; set; } 
        public string UserPhone { get; set; } // رقم الهاتف للمستخدم
        public string UserImage { get; set; } // رابط الصورة الشخصية للمستخدم
        public string RequestedRole { get; set; } //enum لتحديد الدور المطلوب
        public string? Note { get; set; }
        public string? Address { get; set; }

        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // للمندوب فقط
        public string? TargetAgentId { get; set; }
        public string? TargetAgentName { get; set; }
    }
}
