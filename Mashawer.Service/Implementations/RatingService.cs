namespace Mashawer.Service.Implementations
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RatingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> AddRatingAsync(Rating rating, CancellationToken cancellationToken)
        {
            await _unitOfWork.Ratings.AddAsync(rating, cancellationToken);
            await _unitOfWork.CompeleteAsync();
            return "Rating added successfully.";
        }

        public async Task<IEnumerable<RatingDto>> GetRatingsByUserIdAsync(string userId)
        {
            return await _unitOfWork.Ratings.GetTableNoTracking()
                .Where(r => r.RatedUserId == userId)
                .Select(r => new RatingDto
                {
                    Id = r.Id,
                    RatedById = r.RatedById,
                    RatedByName = r.RatedBy.FullName,
                    RatedUserId = r.RatedUserId,
                    RatedUserName = r.RatedUser.FullName,
                    Stars = r.Stars,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                }).ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(string userId)
        {
            var ratings = await _unitOfWork.Ratings.GetTableNoTracking()
                .Where(r => r.RatedUserId == userId)
                .ToListAsync();

            if (!ratings.Any())
                return 0;

            return Math.Round(ratings.Average(r => r.Stars), 2); // Rounded to 2 decimal places
        }

        public async Task<string> DeleteRatingAsync(int ratingId, CancellationToken cancellationToken)
        {
            var rating = await _unitOfWork.Ratings.GetByIdAsync(ratingId);
            if (rating == null)
            {
                return "Rating not found.";
            }

            await _unitOfWork.Ratings.Delete(rating);
            await _unitOfWork.CompeleteAsync();
            return "Rating deleted successfully.";
        }
    }
}