namespace Mashawer.Core.Features.Ratings.Query.Model
{
    public class GetAverageRatingQuery : IRequest<Response<double>>
    {
        public string UserId { get; set; }

        public GetAverageRatingQuery(string userId)
        {
            UserId = userId;
        }
    }
}

