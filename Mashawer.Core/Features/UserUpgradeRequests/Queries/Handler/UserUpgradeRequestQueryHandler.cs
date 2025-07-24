using Mashawer.Core.Features.UserUpgradeRequests.Queries.Models;

namespace Mashawer.Core.Features.UserUpgradeRequests.Queries.Handler
{
    public class UserUpgradeRequestQueryHandler(IUserUpgradeRequestService userUpgradeRequestService) : ResponseHandler,
        IRequestHandler<GetUserUpgradeRequestByIdQuery, Response<UserUpgradeRequest>>,
        IRequestHandler<GetUserUpgradeRequestByAgentIdQuery, Response<IEnumerable<UserUpgradeRequestResponse>>>,
        IRequestHandler<GetUserUpgradeRequestByUserIdQuery, Response<IEnumerable<UserUpgradeRequestResponse>>>,
        IRequestHandler<GetUserUpgradeRequestListQuery, Response<IEnumerable<UserUpgradeRequestResponse>>>,
        IRequestHandler<GetAllUserUpgradeRequestByAddressQuery, Response<IEnumerable<UserUpgradeRequestResponse>>>

    {
        private readonly IUserUpgradeRequestService _userUpgradeRequestService = userUpgradeRequestService;

        public async Task<Response<UserUpgradeRequest>> Handle(GetUserUpgradeRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var response = await _userUpgradeRequestService.GetUpgradeRequestByIdAsync(request.Id);
            if (response == null)
            {
                return NotFound<UserUpgradeRequest>("User upgrade request not found.");
            }
            return Success(response, "User upgrade request retrieved successfully.");

        }

        public async Task<Response<IEnumerable<UserUpgradeRequestResponse>>> Handle(GetUserUpgradeRequestByAgentIdQuery request, CancellationToken cancellationToken)
        {
            var response = await _userUpgradeRequestService.GetUpgradeRequestsByTargetAgentIdAsync(request.AgentId);
            if (response == null || !response.Any())
            {
                return NotFound<IEnumerable<UserUpgradeRequestResponse>>("No user upgrade requests found for the specified agent.");
            }
            return Success(response, "User upgrade requests retrieved successfully.");
        }

        public async Task<Response<IEnumerable<UserUpgradeRequestResponse>>> Handle(GetUserUpgradeRequestByUserIdQuery request, CancellationToken cancellationToken)
        {
            var response = await _userUpgradeRequestService.GetUpgradeRequestsByUserIdAsync(request.UserId);
            if (response == null)
            {
                return (NotFound<IEnumerable<UserUpgradeRequestResponse>>("User upgrade request not found."));
            }
            return Success(response, "User upgrade request retrieved successfully.");
        }

        public async Task<Response<IEnumerable<UserUpgradeRequestResponse>>> Handle(GetUserUpgradeRequestListQuery request, CancellationToken cancellationToken)
        {
            var response = await _userUpgradeRequestService.GetAllUpgradeRequestsAsync();
            if (response == null || !response.Any())
            {
                return NotFound<IEnumerable<UserUpgradeRequestResponse>>("No user upgrade requests found.");
            }
            return Success(response, "User upgrade requests retrieved successfully.");
        }
        public async Task<Response<IEnumerable<UserUpgradeRequestResponse>>> Handle(GetAllUserUpgradeRequestByAddressQuery request, CancellationToken cancellationToken)
        {
            var response = await _userUpgradeRequestService.GetAllUpgradeRequestsByAddressAsync(request.Address);
            return Success(response, "User upgrade requests by address retrieved successfully.");
        }
    }
}
