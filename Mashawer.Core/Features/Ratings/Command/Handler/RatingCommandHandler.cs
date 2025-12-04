using Mashawer.Core.Features.Ratings.Command.Model;

namespace Mashawer.Core.Features.Ratings.Command.Handler
{
    public class RatingCommandHandler : ResponseHandler,
        IRequestHandler<AddRatingCommand, Response<string>>,
        IRequestHandler<DeleteRatingCommand, Response<string>>
    {
        private readonly IRatingService _ratingService;

        public RatingCommandHandler(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        // Handle AddRatingCommand
        public async Task<Response<string>> Handle(AddRatingCommand request, CancellationToken cancellationToken)
        {
            var rating = new Rating
            {
                RatedById = request.RatedById,
                RatedUserId = request.RatedUserId,
                Stars = request.Stars,
                Comment = request.Comment,
            };

            var result = await _ratingService.AddRatingAsync(rating, cancellationToken);
            return Success<string>(result, true);
        }

        // Handle DeleteRatingCommand
        public async Task<Response<string>> Handle(DeleteRatingCommand request, CancellationToken cancellationToken)
        {
            var result = await _ratingService.DeleteRatingAsync(request.RatingId, cancellationToken);
            return Success<string>(result, true);
        }
    }
}