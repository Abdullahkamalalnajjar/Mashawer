
namespace Mashawer.Data.Enums
{
    public enum PaymentMethod
    {
        Cash = 0,             // 💵 الدفع نقدًا عند التسليم
        Visa = 1, 
        AppWallet = 2 ,       // 🪙 الدفع من خلال محفظة التطبيق الداخلية
        LocalWallet = 3,      // 📱 محفظة محلية (زي فودافون كاش، اتصالات كاش، أورنج كاش)
    }
}
