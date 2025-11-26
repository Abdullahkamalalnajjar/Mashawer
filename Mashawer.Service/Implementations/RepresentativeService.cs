namespace Mashawer.Service.Implementations
{
    public class RepresentativeService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, INotificationService notificationService) : IRepresentativeService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly INotificationService _notificationService = notificationService;

        public async Task<IEnumerable<RepresentativeDTO>> GetAllRepresentativesByAddressAsync(string address)
        {
            return await _unitOfWork.Users.GetTableNoTracking()
                .Where(x => x.UserType == UserType.Representative && x.RepresentativeAddress == address)
                .Select(x => new RepresentativeDTO
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    ProfilePictureUrl = x.ProfilePictureUrl,
                    Address = x.RepresentativeAddress,
                    RepresentativeLatitude = x.RepresentativeLatitude,
                    RepresentativeLongitude = x.RepresentativeLongitude
                }).ToListAsync();
        }

        public async Task<IEnumerable<NearestRepresentativeDto>> GetNearestRepresentativeAsync(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude)
        {
            var representatives = await _unitOfWork.Users.GetTableNoTracking()
                .Where(x => x.UserType == UserType.Representative)
                .Select(x => new NearestRepresentativeDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    ProfilePictureUrl = x.ProfilePictureUrl,
                    Address = x.RepresentativeAddress,
                    RepresentativeLatitude = x.RepresentativeLatitude,
                    RepresentativeLongitude = x.RepresentativeLongitude
                }).ToListAsync();

            var deliveryPrice = CalculateDeliveryPrice(fromLatitude, fromLongitude, toLatitude, toLongitude);
            var nearest = representatives
                .Select(rep =>
                {
                    double distance = CalculateDistance(fromLatitude, fromLongitude, (double)rep.RepresentativeLatitude, (double)rep.RepresentativeLongitude);
                    rep.DistanceInKm = Math.Round(distance, 2); // تعيين المسافة داخل DTO
                    rep.DeleveryPrice = deliveryPrice; // تعيين سعر التوصيل داخل DTO
                    return rep;
                })
                .Where(rep => rep.DistanceInKm <= 20).OrderBy(rep => rep.DistanceInKm)
                .Take(10);
            return nearest;
        }

        // حساب المسافة بالكيلومتر بين نقطتين بالإحداثيات
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // نصف قطر الأرض بالكيلومتر
            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        private double DegreesToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }
        private double CalculateDeliveryPrice(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude)
        {
            double distance = CalculateDistance(fromLatitude, fromLongitude, toLatitude, toLongitude);
            double pricePerKm = 5.0; // سعر الكيلو 5 جنيه
            double deliveryPrice = Math.Round(distance * pricePerKm, 2);
            return deliveryPrice;
        }
        public async Task<string> UpdateLocation(string userId, double latitude, double longitude)
        {
            var user = await _unitOfWork.Users.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) return "NotFound";

            user.RepresentativeLatitude = latitude;
            user.RepresentativeLongitude = longitude;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompeleteAsync();

            return "Updated";
        }

        public async Task<string> UpdateInfo(string representativeId, IFormFile vehicalPicture, string vehicalNumber, string type, string vehicleColor)
        {
            var user = await _unitOfWork.Users.GetTableAsTracking().FirstOrDefaultAsync(x => x.Id == representativeId);
            if (user == null) return "NotFound";
            user.VehicleNumber = vehicalNumber;
            user.VehicleType = type;
            user.VehicleColor = vehicleColor;
            user.VehiclePictureUrl = FileHelper.SaveFile(vehicalPicture, "VehicalPicture", _httpContextAccessor);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompeleteAsync();
            return "Updated";
        }
        public async Task<string> MarkDriverArrivedAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetTableAsTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return "NotFound";

            // إرسال إشعار للعميل
            var client = await _unitOfWork.Users.GetTableNoTracking().Where(x => x.Id == order.ClientId).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(client?.FCMToken))
            {
                await _notificationService.SendNotificationAsync(
                    order.ClientId,
                    client.FCMToken,
                    "المندوب قد وصل",
                    "المندوب وصل إلى موقعك الآن، الرجاء الاستعداد للاستلام.",
                    cancellationToken: CancellationToken.None
                );
            }

            return "DriverArrived";
        }
        public async Task<string> MarkIsClientLate(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
                return "NotFound";
            order.IsClientLate = true;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompeleteAsync();
            return "Updated";
        }

        public async Task<RepresentativeInfoDto?> GetRepresentativeInfoAsync(string representativeId)
        {
            return await _unitOfWork.Users.GetTableNoTracking()
                .Select(x => new RepresentativeInfoDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    ProfilePictureUrl = x.ProfilePictureUrl,
                    Address = x.RepresentativeAddress,
                    VehicleNumber = x.VehicleNumber,
                    VehicleType = x.VehicleType,
                    VehicleUrl = x.VehiclePictureUrl,
                    VehicleColor = x.VehicleColor
                })
                .FirstOrDefaultAsync(x => x.Id == representativeId);
        }
    }
}
