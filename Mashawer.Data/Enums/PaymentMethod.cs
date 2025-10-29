using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.Data.Enums
{
    public enum PaymentMethod
    {
        Cash = 0,             // 💵 الدفع نقدًا عند التسليم
        Visa = 1,             // 💳 الدفع الإلكتروني عبر فيزا أو ماستر كارد (من خلال Paymob)
        LocalWallet = 2,      // 📱 محفظة محلية (زي فودافون كاش، اتصالات كاش، أورنج كاش)
        AppWallet = 3         // 🪙 الدفع من خلال محفظة التطبيق الداخلية
    }
}
