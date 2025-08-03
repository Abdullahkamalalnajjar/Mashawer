using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.Service.Implementations
{
    public class OrderService(IUnitOfWork unitOfWork) : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<string> CreateOrderAsync(Order order, CancellationToken cancellationToken)
        {
            await _unitOfWork.Orders.AddAsync(order, cancellationToken);
            return "Created";
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
        {
            return await _unitOfWork.Orders.GetTableNoTracking()
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    FromLatitude = o.FromLatitude,
                    FromLongitude = o.FromLongitude,
                    ToLatitude = o.ToLatitude,
                    ToLongitude = o.ToLongitude,
                    ClientId = o.ClientId,
                    ClientName = o.Client.FullName,
                    CreatedAt = o.CreatedAt,
                    EstimatedArrivalTime = o.EstimatedArrivalTime,
                    PickupLocation = o.PickupLocation,
                    Price = o.Price,
                    DriverId = o.DriverId,
                    DriverName = o.Driver.FullName,
                    VehicleType = o.VehicleType,
                    CancelReason = o.CancelReason.ToString(),
                    Status = o.Status.ToString(),
                    ItemDescription = o.ItemDescription,
                    OtherCancelReasonDetails = o.OtherCancelReasonDetails,
                    DeliveryLocation = o.DeliveryLocation,

                }).ToListAsync();

        }
    }
}
