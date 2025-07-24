
namespace Mashawer.Service.Implementations
{
    public class AdminService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<string> AccpetOrRejectRequestAgentAsync(int requestId, UserType userType, UpgradeRequestStatus upgradeRequestStatus)
        {
            var request = await _unitOfWork.UserUpgradeRequests.GetTableAsTracking()
                 .FirstOrDefaultAsync(x => x.Id == requestId);
            if (request is null)
            {
                return "NotFound";
            }
            var user = await _unitOfWork.Users.GetTableAsTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserId);
            user.UserType = userType; // Assuming the request is for an agent upgrade
            user.AgentAddress = request.Address;
            await _userManager.UpdateAsync(user);
            request.Status = upgradeRequestStatus;
            _unitOfWork.UserUpgradeRequests.Update(request);
            return ("Updated");
        }
    }
}
