using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.Data.Dtos
{
    public class OrderTasksDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; } // 🔗 لمعرفة لأي طلب تتبع

        public string Type { get; set; } // Delivery أو Purchase

        // 📍 المواقع
        public double FromLatitude { get; set; }
        public double FromLongitude { get; set; }
        public double ToLatitude { get; set; }
        public double ToLongitude { get; set; }

        public Address? PickupLocation { get; set; }
        public Address? DeliveryLocation { get; set; }

        // 💰 تفاصيل
        public decimal DeliveryPrice { get; set; }
        public double DistanceKm { get; set; }
        public string? DeliveryDescription { get; set; }

        // 💳 الدفع الفرعي (في حالة المشتريات)
        public bool IsClientPaidForItems { get; set; }
        public bool IsDriverReimbursed { get; set; }

        // 📸 الصور
        public string? ItemPhotoBefore { get; set; }
        public string? ItemPhotoAfter { get; set; }

        // ⚙️ الحالة
        public string Status { get; set; }

        // 🛒 عناصر المشتريات
        public List<PurchaseItemDto> PurchaseItems { get; set; } = new();
    }

}
