using Mashawer.Data.Enums;

namespace Mashawer.Service.Abstracts
{
    public interface IAdminService
    {
        public Task<string> AccpetOrRejectRequestAgentAsync(int requestId, UpgradeRequestStatus upgradeRequestStatus);
    }
}
