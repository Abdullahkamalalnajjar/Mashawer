using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.Orders.Commands.Models
{
    public class CreateOrderCommand : IRequest<Response<string>>
    {
        // 🔹 معلومات عامة
        public string ClientId { get; set; }             // معرف العميل
        public string VehicleType { get; set; }          // نوع المركبة المطلوبة (دراجة / سيارة)
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.NotPaid;
        public bool IsWalletUsed { get; set; } = false;
        public string? PaymobTransactionId { get; set; }

        // 🔹 الطلبات الفرعية (Stops)
        public List<OrderStepDto> Orders { get; set; } = new(); // كل عنصر يمثل مرحلة (توصيل أو مشتريات)
    }

    public class OrderStepDto
    {
        public OrderType Type { get; set; } = OrderType.Delivery; // نوع الطلب (توصيل / مشتريات)

        // 📍 المواقع
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }
        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }

        public Address? PickupLocation { get; set; }
        public Address? DeliveryLocation { get; set; }

        // 🛍️ إعدادات خاصة بالمشتريات
        public bool IsClientPaidForItems { get; set; } = true;
        public bool IsDriverReimbursed { get; set; } = false;

        // 📝 وصف العنصر
        public string? DeliveryDescription { get; set; }

        // 📸 الصور (اختياري)
        public string? ItemPhotoBefore { get; set; }
        public string? ItemPhotoAfter { get; set; }

        // 🛒 العناصر المشتراة (لو النوع مشتريات)
        public List<PurchaseItemDto>? PurchaseItems { get; set; } = new();
    }

   
}
