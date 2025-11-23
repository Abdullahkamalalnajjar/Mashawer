using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.Service.Implementations
{
    public class RepresentitiveCancelOrderService : IRepresentitiveCancelOrderService
    {
        private readonly IRepresentitiveCancelOrderRepository _repository;

        public RepresentitiveCancelOrderService(IRepresentitiveCancelOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RepresentitiveCancelOrder>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<RepresentitiveCancelOrder?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<string> AddAsync(RepresentitiveCancelOrder representitiveCancelOrder)
        {
            await _repository.AddAsync(representitiveCancelOrder);
            return "Created";
        }

        public async Task<string> UpdateAsync(RepresentitiveCancelOrder representitiveCancelOrder)
        {
            _repository.Update(representitiveCancelOrder);
            return "Updated";
        }

        public async Task<string> DeleteAsync(RepresentitiveCancelOrder representitiveCancelOrder)
        {
            await _repository.Delete(representitiveCancelOrder);
            return "Deleted";
        }
    }
}