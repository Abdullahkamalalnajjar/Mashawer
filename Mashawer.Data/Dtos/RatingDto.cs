namespace Mashawer.Data.Dtos
{
    public class RatingDto
    {
        public int Id { get; set; } // Primary key
        public string RatedById { get; set; } // The user who gave the rating
        public string RatedByName { get; set; } // Name of the user who gave the rating
        public string RatedUserId { get; set; } // The user who is being rated
        public string RatedUserName { get; set; } // Name of the user being rated
        public int Stars { get; set; } // Rating value (e.g., 1-5 stars)
        public string? Comment { get; set; } // Optional comment for the rating
        public DateTime CreatedAt { get; set; } // Timestamp for when the rating was created
    }
}