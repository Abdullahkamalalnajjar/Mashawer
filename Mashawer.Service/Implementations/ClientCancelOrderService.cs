using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.Service.Implementations
{
    public class ClientCancelOrderService : IClientCancelOrderService
    {
        private readonly IClientCancelOrderRepository _repository;

        public ClientCancelOrderService(IClientCancelOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ClientCancelOrder>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ClientCancelOrder?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<string> AddAsync(ClientCancelOrder clientCancelOrder)
        {
            await _repository.AddAsync(clientCancelOrder);
            return "Created";
        }

        public async Task<string> UpdateAsync(ClientCancelOrder clientCancelOrder)
        {
            _repository.Update(clientCancelOrder);
            return "Updated";
        }

        public async Task<string> DeleteAsync(ClientCancelOrder clientCancelOrder)
        {
            await _repository.Delete(clientCancelOrder);
            return "Deleted";
        }
    }
}