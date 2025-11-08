using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.Orders.Commands.Models
{
    public class CreateOrderCommand : IRequest<Response<string>>
    {
        // 🔹 معلومات عامة
        public string ClientId { get; set; }             // معرف العميل
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.NotPaid;
        public bool IsWalletUsed { get; set; } = false;
        public string? PaymobTransactionId { get; set; }

        // 🔹 الطلبات الفرعية (Stops)
        public List<OrderTaskDto> Tasks { get; set; } = new(); // كل عنصر يمثل مرحلة (توصيل أو مشتريات)
    }

    public class OrderTaskDto
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
        public double DistanceKm { get; set; } = 0;    

        // 🛒 العناصر المشتراة (لو النوع مشتريات)
        public List<PurchaseItemsDto>? PurchaseItems { get; set; } = new();
    }
    public class PurchaseItemsDto
    {
        public string Name { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }


    }



}
