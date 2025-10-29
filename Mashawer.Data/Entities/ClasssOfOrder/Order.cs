//using Mashawer.Data.Enums;

//namespace Mashawer.Data.Entities.ClasssOfOrder
//{
//    public class Order
//    {
//        public int Id { get; set; }

//        // إحداثيات الموقع على الخريطة
//        public double FromLatitude { get; set; }              // خط العرض من الموقع الحالي
//        public double FromLongitude { get; set; }             // خط الطول من الموقع الحالي

//        public double ToLatitude { get; set; }                // خط العرض إلى الموقع المطلوب
//        public double ToLongitude { get; set; }               // خط الطول إلى الموقع المطلوب

//        // موقع الاستلام
//        public Address PickupLocation { get; set; }

//        // موقع التسليم
//        public Address DeliveryLocation { get; set; }

//        // تفاصيل العنصر المطلوب توصيله
//        public string ItemDescription { get; set; }

//        // تفاصيل الطلب
//        public decimal Price { get; set; }
//        public decimal? PriceAfterDeducation { get; set; } // السعر بعد خصم نسبة التطبيق (إن وجدت) 
//        public string VehicleType { get; set; } // مثل: موتوسيكل
//        public DateTime EstimatedArrivalTime { get; set; }

//        // حالة الطلب
//        public OrderStatus Status { get; set; } = OrderStatus.Pending;

//        // سبب الإلغاء (إن وجد)
//        public CancelReason? CancelReason { get; set; }
//        public string? OtherCancelReasonDetails { get; set; }
//        public string? ItemPhotoBefore { get; set; }
//        public string? ItemPhotoAfter { get; set; }

//        // تاريخ ووقت إنشاء الطلب
//        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//        public string ClientId { get; set; } // معرف المستخدم الذي قام بإنشاء الطلب
//        public ApplicationUser Client { get; set; } // المستخدم الذي قام بإنشاء الطلب
//        public string? DriverId { get; set; } // معرف السائق الذي تم تعيينه للطلب (إن وجد)
//        public ApplicationUser? Driver { get; set; } // السائق الذي تم تعيينه للطلب (إن وجد)

//    }
//}


using Mashawer.Data.Enums;

namespace Mashawer.Data.Entities.ClasssOfOrder
{
    public class Order
    {
        public int Id { get; set; } // رقم تعريف الطلب في قاعدة البيانات

        public OrderType Type { get; set; } = OrderType.Delivery;  // نوع الطلب: توصيل أو مشتريات

        // 📍 إحداثيات المواقع
        public double FromLatitude { get; set; }    // خط العرض لموقع الاستلام
        public double FromLongitude { get; set; }   // خط الطول لموقع الاستلام
        public double ToLatitude { get; set; }      // خط العرض لموقع التسليم
        public double ToLongitude { get; set; }     // خط الطول لموقع التسليم

        public Address PickupLocation { get; set; }     // عنوان موقع الاستلام
        public Address DeliveryLocation { get; set; }   // عنوان موقع التسليم

        // 📦 تفاصيل العنصر أو المشتريات
        public string ItemDescription { get; set; }         // وصف العنصر المراد توصيله
        public string? PurchaseDetails { get; set; }         // تفاصيل المشتريات (لو نوع الطلب مشتريات)

        // 🛍️ إعدادات خاصة بالمشتريات
        public bool IsClientPaidForItems { get; set; } = true;   // لو العميل دفع تمن المشتريات مسبقًا = true، لو المندوب هيدفع مؤقتًا = false
        public decimal? ItemsTotalCost { get; set; }             // إجمالي تمن المشتريات (لو موجود)
        public bool IsDriverReimbursed { get; set; } = false;    // هل المندوب تم تعويضه من العميل أو النظام بعد الدفع؟

        // 💰 الأسعار
        public decimal DeliveryPrice { get; set; }               // سعر التوصيل فقط
        public decimal TotalPrice => (ItemsTotalCost ?? 0) + DeliveryPrice; // السعر الكلي = المشتريات + التوصيل

        // 💳 معلومات الدفع
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;  // طريقة الدفع (كاش، Paymob، محفظة التطبيق)
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.NotPaid; // حالة الدفع (مدفوع، غير مدفوع، تحت المعالجة)
        public string? PaymobTransactionId { get; set; }        // رقم العملية لو الدفع تم عبر Paymob
        public bool IsWalletUsed { get; set; } = false;         // لو الدفع تم باستخدام محفظة التطبيق

        // 🚗 تفاصيل إضافية
        public string VehicleType { get; set; }                 // نوع المركبة المطلوبة (دراجة، سيارة... إلخ)
        public OrderStatus Status { get; set; } = OrderStatus.Pending; // حالة الطلب الحالية (قيد الانتظار، قيد التنفيذ...)
        public string? CancelReason { get; set; }
        public string? OtherCancelReasonDetails { get; set; }


        // 📸 صور العنصر
        public string? ItemPhotoBefore { get; set; }            // صورة العنصر قبل التوصيل أو الشراء
        public string? ItemPhotoAfter { get; set; }             // صورة العنصر بعد التسليم أو التوصيل

        // 👤 معلومات المستخدمين
        public string ClientId { get; set; }                    // رقم تعريف العميل اللي أنشأ الطلب
        public ApplicationUser Client { get; set; }             // الكيان الكامل للعميل
        public string? DriverId { get; set; }                   // رقم تعريف المندوب اللي تم تعيينه
        public ApplicationUser? Driver { get; set; }            // الكيان الكامل للمندوب (لو تم التعيين)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // تاريخ ووقت إنشاء الطلب
    }
}

