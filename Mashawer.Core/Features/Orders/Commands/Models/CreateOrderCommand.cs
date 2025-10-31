using Mashawer.Data.Entities.ClasssOfOrder;
using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.Orders.Commands.Models
{
    public class CreateOrderCommand : IRequest<Response<string>>
    {
        public OrderType Type { get; set; } = OrderType.Delivery; // توصيل أو مشتريات

        // 📍 الإحداثيات
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }
        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }

        // 🏠 المواقع
        public Address PickupLocation { get; set; }
        public Address DeliveryLocation { get; set; }
        // 🛍️ إعدادات المشتريات
        public bool IsClientPaidForItems { get; set; } = true;
        public decimal? ItemsTotalCost { get; set; }
        public bool IsDriverReimbursed { get; set; } = false;

        // 💰 الأسعار
        public decimal DeliveryPrice { get; set; }

        // 💳 الدفع
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;  // كاش، فيزا، محفظة محلية، محفظة التطبيق
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.NotPaid;
        public string? PaymobTransactionId { get; set; }
        public bool IsWalletUsed { get; set; } = false;

        // 🚗 تفاصيل إضافية
        public string VehicleType { get; set; }
        public string ClientId { get; set; }
       
        public List<PrushaseItemDto>? PurchaseItems { get; set; }


    }
    public class PrushaseItemDto
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
