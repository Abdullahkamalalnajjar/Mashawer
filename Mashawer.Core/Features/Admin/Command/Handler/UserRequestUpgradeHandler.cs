using Mashawer.Core.Features.Admin.Command.Models;

namespace Mashawer.Core.Features.Admin.Command.Handler
{
    public class UserRequestUpgradeHandler(IAdminService adminService) : ResponseHandler,
        IRequestHandler<AcceptOrRejectRequestCommand, Response<string>>
    {
        private readonly IAdminService _adminService = adminService;

        public async Task<Response<string>> Handle(AcceptOrRejectRequestCommand request, CancellationToken cancellationToken)
        {
            var result = await _adminService.AccpetOrRejectRequestAgentAsync(request.RequestId, request.Status);
            if (result == "NotFound")
            {
                return NotFound<string>("Request not found.");
            }
            return Success(result, "Request updated successfully.");
        }
    }
}
