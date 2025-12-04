using Mashawer.Core.Features.Ratings.Query.Model;

namespace Mashawer.Core.Features.Ratings.Query.Handler
{
    public class RatingQueryHandler : ResponseHandler,
        IRequestHandler<GetRatingsByUserIdQuery, Response<IEnumerable<RatingDto>>>,
        IRequestHandler<GetAverageRatingQuery, Response<double>>
    {
        private readonly IRatingService _ratingService;

        public RatingQueryHandler(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        public async Task<Response<IEnumerable<RatingDto>>> Handle(GetRatingsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var ratings = await _ratingService.GetRatingsByUserIdAsync(request.UserId);
            return Success<IEnumerable<RatingDto>>(ratings, true);
        }

        public async Task<Response<double>> Handle(GetAverageRatingQuery request, CancellationToken cancellationToken)
        {
            var averageRating = await _ratingService.GetAverageRatingAsync(request.UserId);
            return Success<double>(averageRating, true);
        }
    }
}