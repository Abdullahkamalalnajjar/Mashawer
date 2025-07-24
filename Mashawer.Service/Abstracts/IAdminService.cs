namespace Mashawer.Service.Abstracts
{
    public interface IAdminService
    {
        public Task<string> AccpetOrRejectRequestAgentAsync(int requestId, UserType userType, UpgradeRequestStatus upgradeRequestStatus);
    }
}
