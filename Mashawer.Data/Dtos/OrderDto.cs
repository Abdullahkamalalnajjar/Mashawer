using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;

namespace Mashawer.Data.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }

        // 🧾 نوع الطلب (توصيل / مشتريات)
        public string Type { get; set; }

        // 📍 إحداثيات المواقع
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }
        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }

        public Address PickupLocation { get; set; }
        public Address DeliveryLocation { get; set; }

      
       // 💰 الأسعار
        public decimal DeliveryPrice { get; set; }             // سعر التوصيل
        public decimal TotalPrice { get; set; }                // المجموع الكلي (توصيل + مشتريات)
        public bool IsClientPaidForItems { get; set; }         // هل العميل دفع تمن المشتريات مسبقاً؟
        public bool IsDriverReimbursed { get; set; }           // هل تم تعويض المندوب؟
  

        // 💳 معلومات الدفع
        public string PaymentMethod { get; set; }              // كاش / Paymob / Wallet
        public string PaymentStatus { get; set; }              // مدفوع / غير مدفوع
        public string? PaymobTransactionId { get; set; }       // رقم عملية الدفع (إن وجد)
        public bool IsWalletUsed { get; set; }                 // هل تم الدفع من المحفظة؟

        // 🚗 المركبة
        public string VehicleType { get; set; }                // نوع المركبة المطلوبة
        public string VehicleTypeOfDriver { get; set; }                // نوع المركبة المطلوبة
        public string? VehicleNumber { get; set; }             // رقم المركبة (لو متاح)

        // 📸 الصور
        public string? ItemPhotoBefore { get; set; }
        public string? ItemPhotoAfter { get; set; }

        // ⚙️ الحالة والتواريخ
        public string Status { get; set; }
        public string? CancelReason { get; set; }
        public string? OtherCancelReasonDetails { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 👤 المستخدمين
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientPhoneNumber { get; set; }
        public string? DriverId { get; set; }
        public string? DriverName { get; set; }
        public string? DriverPhoneNumber { get; set; }
        public string? DriverPhotoUrl { get; set; }

        // 📏 المسافة التقريبية (يتم حسابها في الخدمة)
        public double? DistanceKm { get; set; }
        public List<PurchaseItemDto> PurchaseItems { get; set; }

    }
}
