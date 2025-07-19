namespace Mashawer.Service.Abstracts
{
    public interface IUserUpgradeRequestService
    {
        public Task<string> CreateUpgradeRequestAsync(UserUpgradeRequest request, CancellationToken cancellationToken);
        public string UpdateUpgradeRequest(UserUpgradeRequest request);
        public Task<string> DeleteUpgradeRequest (UserUpgradeRequest request);
        public Task<UserUpgradeRequest?> GetUpgradeRequestByIdAsync(int id);
        public Task<IEnumerable<UserUpgradeRequestResponse>> GetAllUpgradeRequestsAsync();
        public Task<IEnumerable<UserUpgradeRequestResponse>> GetUpgradeRequestsByUserIdAsync(string userId);
        public Task<IEnumerable<UserUpgradeRequestResponse>> GetUpgradeRequestsByTargetAgentIdAsync(string targetAgentId);

    }
}
