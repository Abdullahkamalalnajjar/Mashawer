

using Mashawer.Data.Enums;

namespace Mashawer.Data.Entities.ClasssOfOrder
{
    public class Order
    {
        public int Id { get; set; } // رقم تعريف الطلب في قاعدة البيانات

        // ✅ ده يحدد نوع الرحلة العامة (توصيل – مشتريات – خليط)
        public OrderType Type { get; set; } = OrderType.Delivery;

        // 💳 معلومات الدفع
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.NotPaid;
        public string? PaymobTransactionId { get; set; }
        public bool IsWalletUsed { get; set; } = false;

        // 👤 معلومات المستخدمين
        public string ClientId { get; set; }
        public ApplicationUser Client { get; set; }
        public string? DriverId { get; set; }
        public ApplicationUser? Driver { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? CancelReason { get; set; }
        public string? OtherCancelReasonDetails { get; set; }
        public bool IsClientLate { get; set; } = false;

        // 💰 الحسابات
        public decimal? DeducationDelivery { get; set; }
        public decimal? TotalDeliveryPrice { get; set; }
        public decimal? TotalPrice { get; set; }

        public double? TotalDistanceKm { get; set; }

        // 🕓 بيانات التوقيت
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🧩 العلاقة الجديدة — المهام داخل الطلب
        public ICollection<OrderTask> Tasks { get; set; } = new List<OrderTask>();

        // 🧮 دالة لحساب الإجمالي من كل المهام
        public void CalcTotalPrice()
        {
            if (Tasks != null && Tasks.Any())
            {
                TotalPrice = Tasks.Sum(t =>
                    t.DeliveryPrice +
                    (t.PurchaseItems != null ? t.PurchaseItems.Sum(p => p.PriceTotal) : 0)
                );
            }
            else
            {
                TotalPrice = 0;
            }
        }
        // calc total delivery price
        public void CalcTotalDeliveryPrice()
        {
            if (Tasks != null && Tasks.Any())
            {
                TotalDeliveryPrice = Tasks.Sum(t => t.DeliveryPrice);
            }
            else
            {
                TotalDeliveryPrice = 0;
            }
        }
    }
}