using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.Service.Abstracts
{
    public interface IClientCancelOrderService
    {
        Task<IEnumerable<ClientCancelOrder>> GetAllAsync();
        Task<ClientCancelOrder?> GetByIdAsync(int id);
        Task<string> AddAsync(ClientCancelOrder clientCancelOrder);
        Task<string> UpdateAsync(ClientCancelOrder clientCancelOrder);
        Task<string> DeleteAsync(ClientCancelOrder clientCancelOrder);
    }
}