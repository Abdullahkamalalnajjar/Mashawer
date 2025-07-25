using Mashawer.Core.Features.Admin.Command.Models;

namespace Mashawer.Core.Features.Admin.Command.Handler
{
    public class UserRequestUpgradeHandler(IAdminService adminService,IUnitOfWork unitOfWork) : ResponseHandler,
        IRequestHandler<AcceptOrRejectRequestCommand, Response<string>>
    {
        private readonly IAdminService _adminService = adminService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Response<string>> Handle(AcceptOrRejectRequestCommand request, CancellationToken cancellationToken)
        {
            var result = await _adminService.AccpetOrRejectRequestAgentAsync(request.RequestId, request.UserType, request.Status);
            if (result == "NotFound")
            {
                return NotFound<string>("Request not found.");
            }
            await _unitOfWork.CompeleteAsync();
            return Success(result, "Request updated successfully.");
        }
    }
}
