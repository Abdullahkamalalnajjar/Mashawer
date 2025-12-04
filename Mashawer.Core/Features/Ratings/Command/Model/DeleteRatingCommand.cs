namespace Mashawer.Core.Features.Ratings.Command.Model
{
    public class DeleteRatingCommand : IRequest<Response<string>>
    {
        public int RatingId { get; set; }

        public DeleteRatingCommand(int ratingId)
        {
            RatingId = ratingId;
        }
    }
}