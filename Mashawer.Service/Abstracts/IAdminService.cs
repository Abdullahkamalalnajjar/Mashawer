namespace Mashawer.Service.Abstracts
{
    public interface IAdminService
    {
        public Task<string> AccpetOrRejectRequestAgentAsync(int requestId, UserType userType, UpgradeRequestStatus upgradeRequestStatus);
        public Task<List<OrderDto>> GetAllOrdersDpendOnStatusAsync(OrderStatus orderStatus, string? address , DateTime? dateTime);

    }
}
