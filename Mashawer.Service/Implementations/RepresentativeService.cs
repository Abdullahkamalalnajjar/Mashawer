using Microsoft.AspNetCore.Mvc;

namespace Mashawer.Service.Implementations
{
    public class RepresentativeService(IUnitOfWork unitOfWork) : IRepresentativeService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

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
            //var order = await _unitOfWork.Orders.GetTableNoTracking()
            //    .Where(x => x.Id == orderId)
            //    .Select(x => new
            //    {
            //        x.ToLatitude,
            //        x.ToLongitude
            //    }).FirstOrDefaultAsync();
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
            var user = await _unitOfWork.Users.GetTableAsTracking().FirstOrDefaultAsync(x=>x.Id==userId);
            if (user == null) return "NotFound";

            user.RepresentativeLatitude = latitude;
            user.RepresentativeLongitude = longitude;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompeleteAsync();

            return "Updated";
        }


    }
}
