
namespace Mashawer.Service.Implementations
{
    public class AdminService(IUnitOfWork unitOfWork) : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<string> AccpetOrRejectRequestAgentAsync(int requestId, UpgradeRequestStatus upgradeRequestStatus)
        {
            var request = await _unitOfWork.UserUpgradeRequests.GetTableAsTracking()
                 .FirstOrDefaultAsync(x => x.Id == requestId);
            if (request is null)
            {
                return "NotFound";
            }
            request.Status = upgradeRequestStatus;
            _unitOfWork.UserUpgradeRequests.Update(request);
            return ("Updated");
        }
    }
}
