using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.Core.Features.Orders.Commands.Models
{
    public class CreateOrderCommand : IRequest<Response<string>>
    {
        public double FromLatitude { get; set; }              // خط العرض من الموقع الحالي
        public double FromLongitude { get; set; }             // خط الطول من الموقع الحالي
        public double ToLatitude { get; set; }                // خط العرض إلى الموقع المطلوب
        public double ToLongitude { get; set; }               // خط الطول إلى الموقع المطلوب
        // موقع الاستلام
        public Address PickupLocation { get; set; }
        // موقع التسليم
        public Address DeliveryLocation { get; set; }
        // تفاصيل العنصر المطلوب توصيله
        public string ItemDescription { get; set; }
        // تفاصيل الطلب
        public decimal Price { get; set; }
        public string VehicleType { get; set; } // مثل: موتوسيكل
        public DateTime EstimatedArrivalTime { get; set; }
        public string ClientId { get; set; } // معرف المستخدم الذي قام بإنشاء الطلب
                                             //  public string? DriverId { get; set; } // معرف السائق الذي تم تعيينه للطلب (إن وجد)
    }
}
