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
        #region Expression to convert Order to OrderDto
        private static readonly Expression<Func<Order, OrderDto>> OrderToDto = o => new OrderDto
        {
            Id = o.Id,

            // 🧾 نوع الطلب (توصيل / مشتريات)
            Type = o.Type.ToString(),

            // 📍 المواقع
            FromLatitude = o.FromLatitude,
            FromLongitude = o.FromLongitude,
            ToLatitude = o.ToLatitude,
            ToLongitude = o.ToLongitude,
            PickupLocation = o.PickupLocation,
            DeliveryLocation = o.DeliveryLocation,

            // 💰 الأسعار
            DeliveryPrice = o.DeliveryPrice,
            TotalPrice = o.TotalPrice ?? 0,  // لتفادي NullReference
            IsClientPaidForItems = o.IsClientPaidForItems,
            IsDriverReimbursed = o.IsDriverReimbursed,

            // 💳 الدفع
            PaymentMethod = o.PaymentMethod.ToString(),
            PaymentStatus = o.PaymentStatus.ToString(),
            PaymobTransactionId = o.PaymobTransactionId,
            IsWalletUsed = o.IsWalletUsed,

            // 🚗 المركبة
            VehicleType = o.VehicleType,
            VehicleNumber = o.Driver != null ? o.Driver.VehicleNumber : null,
            VehicleTypeOfDriver = o.Driver != null ? o.Driver.VehicleType : null,

            // 👤 المستخدمين
            ClientId = o.ClientId,
            ClientName = o.Client.FullName,
            ClientPhoneNumber = o.Client.PhoneNumber,
            DriverId = o.DriverId,
            DriverName = o.Driver != null ? o.Driver.FullName : null,
            DriverPhoneNumber = o.Driver != null ? o.Driver.PhoneNumber : null,
            DriverPhotoUrl = o.Driver != null ? o.Driver.ProfilePictureUrl : null,

            // 📸 الصور
            ItemPhotoBefore = o.ItemPhotoBefore,
            ItemPhotoAfter = o.ItemPhotoAfter,

            // ⚙️ الحالة والتفاصيل
            Status = o.Status.ToString(),
            CancelReason = o.CancelReason,
            OtherCancelReasonDetails = o.OtherCancelReasonDetails,
            DistanceKm=o.DistanceKm,
            

            // 🕒 التاريخ
            CreatedAt = o.CreatedAt,

            // 🧾 عناصر المشتريات (لو نوع الطلب مشتريات)
            PurchaseItems = o.PurchaseItems.Select(p => new PurchaseItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Quantity = p.Quantity,
                PricePerUnit = p.Price,
                PriceTotal = p.PriceTotal
            }).ToList()
        };
        #endregion


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
            double delta = radiusKm / 111; // درجة تقريبية لكل كم
            var minLat = lat - delta;
            var maxLat = lat + delta;
            var minLng = lng - delta;
            var maxLng = lng + delta;

            // ✅ جلب الطلبات القريبة باستخدام Projection مباشر
            var orders = await _unitOfWork.Orders.GetTableNoTracking()
                .Where(o => o.Status == OrderStatus.Pending &&
                            o.FromLatitude >= minLat && o.FromLatitude <= maxLat &&
                            o.FromLongitude >= minLng && o.FromLongitude <= maxLng)
                .Select(OrderToDto) // هنا بنستخدم الـ Expression الجاهز
                .ToListAsync();

            // ✅ حساب المسافة بعدين (لأنها مش محسوبة داخل الـ Expression)
            var nearby = orders
                .Select(o => new
                {
                    Order = o,
                    Distance = HaversineKm(lat, lng, o.FromLatitude, o.FromLongitude)
                })
                .Where(x => x.Distance <= radiusKm)
                .OrderBy(x => x.Distance)
                .Take(take)
                .Select(x =>
                {
                    x.Order.DistanceKm = Math.Round(x.Distance, 2);
                    return x.Order;
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

