namespace Mashawer.Service.Abstracts
{
    public interface IAgentService
    {
        public Task<List<OrderDto>> GetOrdersByAgentAddressAsync(string userId, OrderStatus orderStatus, DateTime? dateTime);

    }
}
