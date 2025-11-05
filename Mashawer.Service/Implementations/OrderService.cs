using Mashawer.Data.Entities.ClasssOfOrder;
using System.Linq.Expressions;

namespace Mashawer.Service.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<string> CreateOrderAsync(Order order, CancellationToken cancellationToken)
        {
            await _unitOfWork.Orders.AddAsync(order, cancellationToken);
            return "Created";
        }

        #region Expression: Convert Order to OrderDto
        private static readonly Expression<Func<Order, OrderDto>> OrderToDto = o => new OrderDto
        {
            Id = o.Id,
            Type = o.Type.ToString(),

            // 💳 الدفع
            PaymentMethod = o.PaymentMethod.ToString(),
            PaymentStatus = o.PaymentStatus.ToString(),
            PaymobTransactionId = o.PaymobTransactionId,
            IsWalletUsed = o.IsWalletUsed,

            // 🚗 تفاصيل المركبة
            VehicleTypeOfDriver = o.Driver != null ? o.Driver.VehicleType : null,
            VehicleNumber = o.Driver != null ? o.Driver.VehicleNumber : null,

            // 👤 المستخدمين
            ClientId = o.ClientId,
            ClientName = o.Client.FullName,
            ClientPhoneNumber = o.Client.PhoneNumber,
            DriverId = o.DriverId,
            DriverName = o.Driver != null ? o.Driver.FullName : null,
            DriverPhoneNumber = o.Driver != null ? o.Driver.PhoneNumber : null,
            DriverPhotoUrl = o.Driver != null ? o.Driver.ProfilePictureUrl : null,

            // ⚙️ الحالة
            Status = o.Status.ToString(),
            CancelReason = o.CancelReason,
            OtherCancelReasonDetails = o.OtherCancelReasonDetails,
            CreatedAt = o.CreatedAt,
            IsClientLate = o.IsClientLate,

            // 💰 المجموع العام
            TotalPrice = o.TotalPrice ?? 0,
            DeducationDelivery = o.DeducationDelivery ?? 0,
            DistanceKm = o.TotalDistanceKm,

            // 🧩 المهام داخل الطلب
            Tasks = o.Tasks.Select(t => new OrderTasksDto
            {
                Id = t.Id,
                Type = t.Type.ToString(),
                FromLatitude = t.FromLatitude,
                FromLongitude = t.FromLongitude,
                ToLatitude = t.ToLatitude,
                ToLongitude = t.ToLongitude,
                PickupLocation = t.PickupLocation,
                DeliveryLocation = t.DeliveryLocation,
                DeliveryPrice = t.DeliveryPrice,
                DistanceKm = (double)t.DistanceKm,
                DeliveryDescription = t.DeliveryDescription,
                IsClientPaidForItems = t.IsClientPaidForItems,
                IsDriverReimbursed = t.IsDriverReimbursed,
                ItemPhotoBefore = t.ItemPhotoBefore,
                ItemPhotoAfter = t.ItemPhotoAfter,
                Status = t.Status.ToString(),
                PurchaseItems = t.PurchaseItems.Select(p => new PurchaseItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Quantity = p.Quantity,
                    Price = p.Price,
                    PriceTotal = p.PriceTotal
                }).ToList()
            }).ToList()
        };
        #endregion

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
        {
            return await _unitOfWork.Orders.GetTableNoTracking()
                .Include(o => o.Client)
                .Include(o => o.Driver)
                .Include(o => o.Tasks)
                    .ThenInclude(t => t.PurchaseItems)
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
                .Include(o => o.Tasks)
                    .ThenInclude(t => t.PurchaseItems)
                .Where(o => o.ClientId == clientId)
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
                .Include(o => o.Tasks)
                    .ThenInclude(t => t.PurchaseItems)
                .Where(o => o.DriverId == driverId)
                .Select(OrderToDto)
                .ToListAsync();
        }

        public async Task<string> AddOrderPhotosAsync(int orderTaskId, string? photoBefore, string? photoAfter, CancellationToken cancellationToken)
        {
            var task = await _unitOfWork.OrderTasks.GetByIdAsync(orderTaskId);
            if (task == null)
                return "Order task not found";

            if (!string.IsNullOrEmpty(photoBefore))
                task.ItemPhotoBefore = photoBefore;

            if (!string.IsNullOrEmpty(photoAfter))
                task.ItemPhotoAfter = photoAfter;

            _unitOfWork.OrderTasks.Update(task);
            await _unitOfWork.CompeleteAsync();

            return "Task photos updated successfully";
        }

        public async Task<IEnumerable<OrderDto>> GetNearbyPendingOrdersAsync(double lat, double lng, double radiusKm, int take)
        {
            double delta = radiusKm / 111; // درجة تقريبية لكل كم
            var minLat = lat - delta;
            var maxLat = lat + delta;
            var minLng = lng - delta;
            var maxLng = lng + delta;

            var orders = await _unitOfWork.Orders.GetTableNoTracking()
                .Include(o => o.Tasks)
                .Where(o => o.Status == OrderStatus.Pending &&
                            o.Tasks.Any(t =>
                                t.FromLatitude >= minLat && t.FromLatitude <= maxLat &&
                                t.FromLongitude >= minLng && t.FromLongitude <= maxLng))
                .Select(OrderToDto)
                .ToListAsync();

            var nearby = orders
                .Select(o =>
                {
                    var firstTask = o.Tasks.FirstOrDefault();
                    if (firstTask == null) return null;

                    var dist = HaversineKm(lat, lng, firstTask.FromLatitude, firstTask.FromLongitude);
                    o.DistanceKm = Math.Round(dist, 2);
                    return o;
                })
                .Where(o => o != null && o.DistanceKm <= radiusKm)
                .OrderBy(o => o.DistanceKm)
                .Take(take)
                .ToList()!;

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
            if (order != null)
            {
                order.Status = OrderStatus.Completed;
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.CompeleteAsync();
            }
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            return await _unitOfWork.Orders.GetTableNoTracking()
                .Include(o => o.Client)
                .Include(o => o.Driver)
                .Include(o => o.Tasks)
                    .ThenInclude(t => t.PurchaseItems)
                .Where(o => o.Id == orderId)
                .Select(OrderToDto)
                .FirstOrDefaultAsync();
        }
    }
}
