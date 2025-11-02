// GeoHelper.cs
using System;

namespace Mashawer.Data.Helpers
{
    public static class GeoHelper
    {
        // حساب المسافة بالكيلومتر بين نقطتين
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
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

        private static double DegreesToRadians(double deg) => deg * (Math.PI / 180);

        // حساب سعر التوصيل مباشرة كـ decimal
        public static decimal CalculateDeliveryPrice(double fromLat, double fromLon, double toLat, double toLon)
        {
            double distance = CalculateDistance(fromLat, fromLon, toLat, toLon);
            decimal pricePerKm = 5.0m;
            return Math.Round((decimal)distance * pricePerKm, 2);
        }
    }
}
