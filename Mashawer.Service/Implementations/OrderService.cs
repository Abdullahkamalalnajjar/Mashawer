using Mashawer.Data.Entities.ClasssOfOrder;
using System.Linq.Expressions;

namespace Mashawer.Service.Implementations
{
    public class OrderService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager) : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<string> CreateOrderAsync(Order order, CancellationToken cancellationToken)
        {
            await _unitOfWork.Orders.AddAsync(order, cancellationToken);
            return "Created";
        }

        #region  Expression to convert Order to OrderDto
        private static readonly Expression<Func<Order, OrderDto>> OrderToDto = o => new OrderDto
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
            DriverImageUrl = o.Driver != null ? o.Driver.ProfilePictureUrl : null,
            VehicleNumber = o.Driver != null ? o.Driver.VehicleNumber : null,
            VehicleTypeOfDriver = o.Driver != null ? o.Driver.VehicleType : null,
            DriverPhoneNumber = o.Driver != null ? o.Driver.PhoneNumber : null,
            DriverPhotoUrl = o.Driver != null ? o.Driver.ProfilePictureUrl : null,
            VehicleType = o.VehicleType,
            CancelReason = o.CancelReason.ToString(),
            Status = o.Status.ToString(),
            OtherCancelReasonDetails = o.OtherCancelReasonDetails,
            DeliveryLocation = o.DeliveryLocation,
            ItemPhotoAfter = o.ItemPhotoAfter,
            ItemPhotoBefore = o.ItemPhotoBefore,
            ItemDescription = o.ItemDescription

        };

        #endregion 
        public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
        {
            return await _unitOfWork.Orders.GetTableNoTracking()
                 .Include(o => o.Client)
                 .Include(o => o.Driver)
                .Select(OrderToDto)
                .ToListAsync();

        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByClientIdAsync(string clientId)
        {
            var client = await _userManager.FindByIdAsync(clientId);
            if (client == null)
                return Enumerable.Empty<OrderDto>();
            return await _unitOfWork.Orders.GetTableNoTracking()
                .Include(o => o.Client)
                 .Include(o => o.Driver)
               .Where(x => x.ClientId == clientId)
               .Select(OrderToDto)
               .ToListAsync();

        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByDriverIdAsync(string driverId)
        {
            var driver = await _userManager.FindByIdAsync(driverId);
            if (driver == null)
                return Enumerable.Empty<OrderDto>();
            return await _unitOfWork.Orders.GetTableNoTracking()
                .Include(o => o.Client)
                 .Include(o => o.Driver)
              .Where(x => x.DriverId == driverId)
              .Select(OrderToDto)
              .ToListAsync();
        }

        public async Task<string> AddOrderPhotosAsync(int orderId, string? photoBefore, string? photoAfter, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                return "Order not found";

            if (!string.IsNullOrEmpty(photoBefore))
                order.ItemPhotoBefore = photoBefore;

            if (!string.IsNullOrEmpty(photoAfter))
                order.ItemPhotoAfter = photoAfter;

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompeleteAsync();

            return "Photos updated successfully";
        }

        public async Task<IEnumerable<OrderDto>> GetNearbyPendingOrdersAsync(double lat, double lng, double radiusKm, int take)
        {
            // هات pending + البيانات اللازمة للـ DTO
            var pending = await _unitOfWork.Orders.GetTableNoTracking()
                .Where(o => o.Status == OrderStatus.Pending)
                .Include(o => o.Client)
                .Include(o => o.Driver)
                .ToListAsync();

            var nearby = pending
                .Select(o => new
                {
                    Order = o,
                    Distance = HaversineKm(lat, lng, o.FromLatitude, o.FromLongitude)
                })
                .Where(x => x.Distance <= radiusKm)
                .OrderBy(x => x.Distance)
                .Take(take)
                .Select(x => new OrderDto
                {
                    Id = x.Order.Id,
                    FromLatitude = x.Order.FromLatitude,
                    FromLongitude = x.Order.FromLongitude,
                    ToLatitude = x.Order.ToLatitude,
                    ToLongitude = x.Order.ToLongitude,
                    PickupLocation = x.Order.PickupLocation,
                    DeliveryLocation = x.Order.DeliveryLocation,
                    ItemDescription = x.Order.ItemDescription,
                    Price = x.Order.Price,
                    VehicleType = x.Order.VehicleType,
                    EstimatedArrivalTime = x.Order.EstimatedArrivalTime,
                    Status = x.Order.Status.ToString(),
                    CreatedAt = x.Order.CreatedAt,
                    ClientId = x.Order.ClientId,
                    ClientName = x.Order.Client.FullName,
                    DriverId = x.Order.DriverId,
                    DriverName = x.Order.Driver?.FullName,
                    DriverPhoneNumber = x.Order.Driver?.PhoneNumber,
                    DriverPhotoUrl = x.Order.Driver?.ProfilePictureUrl,
                    ItemPhotoAfter = x.Order.ItemPhotoAfter,
                    ItemPhotoBefore = x.Order.ItemPhotoBefore,
                    VehicleNumber = x.Order.Driver.VehicleNumber,
                    VehicleTypeOfDriver = x.Order.Driver.VehicleType,
                })
                .ToList();

            return nearby;
        }

        private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0;
            double dLat = ToRad(lat2 - lat1);
            double dLon = ToRad(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        private static double ToRad(double deg) => deg * (Math.PI / 180.0);

        public async Task TestingAsync()
        {
            var order = await _unitOfWork.Orders.GetTableAsTracking()
                .FirstOrDefaultAsync(x => x.Id == 61);
            order!.Status = OrderStatus.Completed;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompeleteAsync();

        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            return await _unitOfWork.Orders.GetTableNoTracking()
                .Where(x => x.Id == orderId)
                .Select(OrderToDto)
                .FirstOrDefaultAsync();
        }
    }
}

