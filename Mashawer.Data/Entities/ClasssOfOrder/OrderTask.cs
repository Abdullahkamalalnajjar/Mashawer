using Mashawer.Data.Enums;

namespace Mashawer.Data.Entities.ClasssOfOrder
{
    public class OrderTask
    {
        public int Id { get; set; }


        public OrderType Type { get; set; } = OrderType.Delivery;

        // 📍 الإحداثيات الخاصة بالمهمة
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }
        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }

        public Address? PickupLocation { get; set; }
        public Address? DeliveryLocation { get; set; }

        // 💰 السعر الخاص بالمهمة
        public decimal DeliveryPrice { get; set; }
        public double DistanceKm { get; set; }

        // 📝 تفاصيل إضافية
        public string? DeliveryDescription { get; set; }

        // 🛍️ إعدادات خاصة بالمشتريات
        public bool IsClientPaidForItems { get; set; } = true;
        public bool IsDriverReimbursed { get; set; } = false;

        // 📸 الصور
        public string? ItemPhotoBefore { get; set; }
        public string? ItemPhotoAfter { get; set; }

        // 💬 الحالة الحالية للمهمة
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // 🛒 لو المهمة دي مشتريات
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public ICollection<PurchaseItem>? PurchaseItems { get; set; } = new List<PurchaseItem>();

    }
}
