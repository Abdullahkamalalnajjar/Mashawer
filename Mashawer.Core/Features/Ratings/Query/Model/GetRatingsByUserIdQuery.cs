namespace Mashawer.Core.Features.Ratings.Query.Model
{
    public class GetRatingsByUserIdQuery : IRequest<Response<IEnumerable<RatingDto>>>
    {
        public string UserId { get; set; }

        public GetRatingsByUserIdQuery(string userId)
        {
            UserId = userId;
        }
    }
}