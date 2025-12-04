using Mashawer.Data.Dtos;

namespace Mashawer.Core.Features.Ratings.Command.Model
{
    public class AddRatingCommand : IRequest<Response<string>>
    {
        public string RatedById { get; set; }
        public string RatedUserId { get; set; }
        public int Stars { get; set; }
        public string? Comment { get; set; }
    }
}