using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.Data.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }

        // 🧾 نوع الطلب (عام — يحتوي على Tasks)
        public string Type { get; set; }

        // 💰 الأسعار
        public decimal DeliveryPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DeducationDelivery { get; set; }

        public bool IsClientPaidForItems { get; set; }
        public bool IsDriverReimbursed { get; set; }

        // 💳 الدفع
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string? PaymobTransactionId { get; set; }
        public bool IsWalletUsed { get; set; }

        // 🚗 المركبة
        public string VehicleType { get; set; } // المطلوبة
        public string? VehicleTypeOfDriver { get; set; } // نوع مركبة السائق
        public string? VehicleNumber { get; set; }

        // 📸 الصور
        public string? ItemPhotoBefore { get; set; }
        public string? ItemPhotoAfter { get; set; }

        // ⚙️ الحالة والتواريخ
        public string Status { get; set; }
        public string? CancelReason { get; set; }
        public string? OtherCancelReasonDetails { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsClientLate { get; set; }

        // 👤 المستخدمين
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientPhoneNumber { get; set; }

        public string? DriverId { get; set; }
        public string? DriverName { get; set; }
        public string? DriverPhoneNumber { get; set; }
        public string? DriverPhotoUrl { get; set; }

        // 📏 المسافة
        public double? DistanceKm { get; set; }

        // 🧾 التفاصيل الفرعية
        public List<OrderTaskDto> Tasks { get; set; } = new();
    }

}
