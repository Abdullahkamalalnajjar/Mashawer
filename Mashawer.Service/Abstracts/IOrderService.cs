using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.Service.Abstracts
{
    public interface IOrderService
    {
        public Task<string> CreateOrderAsync(Order order, CancellationToken cancellationToken);
        public Task<IEnumerable<OrderDto>> GetOrdersAsync();
    }
}
