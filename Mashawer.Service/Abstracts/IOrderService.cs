using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.Service.Abstracts
{
    public interface IOrderService
    {
        public Task<string> CreateOrderAsync(Order order, CancellationToken cancellationToken);
        public Task<IEnumerable<OrderDto>> GetOrdersAsync();
        public Task<IEnumerable<OrderDto>> GetOrdersByClientIdAsync(string clientId);
        public Task<IEnumerable<OrderDto>> GetOrdersByDriverIdAsync(string driverId);
        public Task<string> AddOrderPhotosAsync(int orderId, string? photoBefore, string? photoAfter, CancellationToken cancellationToken);


    }
}
