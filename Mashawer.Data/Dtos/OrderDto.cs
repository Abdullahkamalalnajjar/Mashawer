using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.Data.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }

        // إحداثيات الموقع على الخريطة
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

        // رقم هاتف المرسل


        // تفاصيل الطلب
        public decimal Price { get; set; }
        public string VehicleType { get; set; } // مثل: موتوسيكل
        public DateTime EstimatedArrivalTime { get; set; }

        // حالة الطلب
        public string Status { get; set; }

        // سبب الإلغاء (إن وجد)
        public string? CancelReason { get; set; }
        public string? OtherCancelReasonDetails { get; set; }
        // تاريخ ووقت إنشاء الطلب
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string ClientId { get; set; } // معرف المستخدم الذي قام بإنشاء الطلب
        public string ClientName { get; set; } // المستخدم الذي قام بإنشاء الطلب
        public string? DriverId { get; set; } // معرف السائق الذي تم تعيينه للطلب (إن وجد)
        public string? DriverName { get; set; } // السائق الذي تم تعيينه للطلب (إن وجد)
        public string? ItemPhotoBefore { get; set; }
        public string? ItemPhotoAfter { get; set; }
    }
}
