using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.Service.Abstracts
{
    public interface IRepresentitiveCancelOrderService
    {
        Task<IEnumerable<RepresentitiveCancelOrder>> GetAllAsync();
        Task<RepresentitiveCancelOrder?> GetByIdAsync(int id);
        Task<string> AddAsync(RepresentitiveCancelOrder representitiveCancelOrder);
        Task<string> UpdateAsync(RepresentitiveCancelOrder representitiveCancelOrder);
        Task<string> DeleteAsync(RepresentitiveCancelOrder representitiveCancelOrder);
    }
}