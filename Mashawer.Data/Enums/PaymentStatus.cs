using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.Data.Enums
{
    public enum PaymentStatus
    {
        NotPaid = 0,     // ❌ لم يتم الدفع بعد
        Pending = 1,     // ⏳ الدفع قيد التنفيذ (مثل أثناء معالجة Paymob)
        Paid = 2,        // ✅ تم الدفع بنجاح
        Failed = 3,      // ⚠️ فشلت عملية الدفع (مثلاً رفض البطاقة أو خطأ من Paymob)
        Refunded = 4     // 🔁 تم استرجاع المبلغ (في حالة إلغاء الطلب بعد الدفع)
    }
}
