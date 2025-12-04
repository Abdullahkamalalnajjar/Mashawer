namespace Mashawer.Data.Entities
{
    public class Rating
    {
        public int Id { get; set; } // Primary key

        public string RatedById { get; set; } // The user who gave the rating
        public ApplicationUser RatedBy { get; set; } // Navigation property for the user who gave the rating

        public string RatedUserId { get; set; } // The user who is being rated
        public ApplicationUser RatedUser { get; set; } // Navigation property for the user being rated

        public int Stars { get; set; } // Rating value (e.g., 1-5 stars)
        public string? Comment { get; set; } // Optional comment for the rating

        // Use Egypt timezone for CreatedAt
        public DateTime CreatedAt { get; set; } = GetEgyptTime();

        // Static method to get the current time in Egypt timezone
        private static DateTime GetEgyptTime()
        {
            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptTimeZone);
        }
    }
}