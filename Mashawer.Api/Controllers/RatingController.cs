using Mashawer.Api.Base;
using Mashawer.Core.Features.Ratings.Command.Model;
using Mashawer.Core.Features.Ratings.Query.Model;
using Mashawer.Data.AppMetaData;
using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Api.Controllers
{
    public class RatingController : AppBaseController
    {
        [HttpPost(Router.RatingRouting.Create)]
        public async Task<IActionResult> AddRating([FromBody] AddRatingCommand command, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(command, cancellationToken);
            return NewResult(result);
        }

        [HttpGet(Router.RatingRouting.GetByUserId)]
        public async Task<IActionResult> GetRatingsByUserId([FromRoute] string userId)
        {
            var query = new GetRatingsByUserIdQuery(userId);
            var result = await Mediator.Send(query);
            return NewResult(result);
        }

        [HttpGet(Router.RatingRouting.GetAverageRating)]
        public async Task<IActionResult> GetAverageRating([FromRoute] string userId)
        {
            var query = new GetAverageRatingQuery(userId);
            var result = await Mediator.Send(query);
            return NewResult(result);
        }

        [HttpDelete(Router.RatingRouting.Delete)]
        public async Task<IActionResult> DeleteRating([FromRoute] int id, CancellationToken cancellationToken)
        {
            var command = new DeleteRatingCommand(id);
            var result = await Mediator.Send(command, cancellationToken);
            return NewResult(result);
        }
    }
}