using Mashawer.Core.Features.UserDailyDiscounts.Command.Models;

namespace Mashawer.Core.Features.UserDailyDiscounts.Command.Handler
{
    public class UserDailyDiscountCommandHandler : ResponseHandler,
     IRequestHandler<AddUserDailyDiscountCommand, Response<string>>,
        IRequestHandler<MarkUsedCommand, Response<string>>,
        IRequestHandler<AddDicscontForAllNormalUserCommand, Response<string>>
    {
        private readonly IUserDailyDiscountService _dailyDiscountService;
        private readonly IUnitOfWork _unit;

        public UserDailyDiscountCommandHandler(
            IUserDailyDiscountService dailyDiscountService,
            IUnitOfWork unit)
        {
            _dailyDiscountService = dailyDiscountService;
            _unit = unit;
        }

        public async Task<Response<string>> Handle(AddUserDailyDiscountCommand request, CancellationToken cancellationToken)
        {
            await _dailyDiscountService.AddDiscountAsync(request.UserId, request.DiscountDate, request.DiscountAmount);

            await _unit.CompeleteAsync();

            return Created("Daily discount added successfully");
        }

        public async Task<Response<string>> Handle(MarkUsedCommand request, CancellationToken cancellationToken)
        {
            var marked = await _dailyDiscountService.MarkUsedAsync(request.Ids);
            if (marked == "MarkedAsUsed")
                return Success("Daily discount marked as used successfully");
            return NotFound<string>("Daily discount not found.");
        }

        public async Task<Response<string>> Handle(AddDicscontForAllNormalUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _dailyDiscountService.AddDiscontAsyncForAllNormalUser(request.DiscountAmount, request.DiscountDate);
            if (result == "Added")
                return Created("Daily discounts added successfully for all normal users.");
            return BadRequest<string>("Failed to add daily discounts for all normal users.");
        }
    }

}
