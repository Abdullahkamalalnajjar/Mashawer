namespace Mashawer.Service.Abstracts
{
    public interface IRatingService
    {
        Task<string> AddRatingAsync(Rating rating, CancellationToken cancellationToken);
        Task<IEnumerable<RatingDto>> GetRatingsByUserIdAsync(string userId);
        Task<double> GetAverageRatingAsync(string userId);
        Task<string> DeleteRatingAsync(int ratingId, CancellationToken cancellationToken); // New method
    }
}