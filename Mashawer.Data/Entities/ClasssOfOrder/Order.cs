using Mashawer.Data.Enums;

namespace Mashawer.Data.Entities.ClasssOfOrder
{
    public class Order
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
        public string SenderPhoneNumber { get; set; }


        // تفاصيل الطلب
        public decimal Price { get; set; }
        public string VehicleType { get; set; } // مثل: موتوسيكل
        public DateTime EstimatedArrivalTime { get; set; }

        // حالة الطلب
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // سبب الإلغاء (إن وجد)
        public CancelReason? CancelReason { get; set; }
        public string? OtherCancelReasonDetails { get; set; }
        // تاريخ ووقت إنشاء الطلب
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string ClientId { get; set; } // معرف المستخدم الذي قام بإنشاء الطلب
        public ApplicationUser Client { get; set; } // المستخدم الذي قام بإنشاء الطلب
        public string? DriverId { get; set; } // معرف السائق الذي تم تعيينه للطلب (إن وجد)
        public ApplicationUser? Driver { get; set; } // السائق الذي تم تعيينه للطلب (إن وجد)

    }
}
