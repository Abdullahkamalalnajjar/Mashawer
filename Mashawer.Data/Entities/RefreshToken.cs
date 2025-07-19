﻿namespace Mashawer.Data.Entities
{

    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public bool IsActive => RevokedOn is null && !IsExpired;
        public string ApplicationUserId { get; set; } 
        public ApplicationUser ApplicationUser { get; set; } 

    }
}
