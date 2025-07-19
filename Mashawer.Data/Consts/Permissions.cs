namespace Mashawer.Data.Consts
{
    public static class Permissions
    {
        public static string Type { get; } = "permissions"; //properity

        // إذن قراءة وعرض السيريال كودز
        public const string GetSerialCodes = "serialcodes:read";

        // إذن إضافة سيريال كودز جديدة
        public const string AddSerialCodes = "serialcodes:add";

        // إذن تحديث وتعديل السيريال كودز الموجودة
        public const string UpdateSerialCodes = "serialcodes:update";

        // إذن حذف السيريال كودز
        public const string DeleteSerialCodes = "serialcodes:delete";

        // إذن استخدام وتفعيل السيريال كود
        public const string UseSerialCode = "serialcodes:use";

        // إذن قراءة وعرض خطط المستخدمين
        public const string GetUserPlans = "users:plans:read";

        // إذن ترقية خطة المستخدم إلى خطة أعلى
        public const string UpgradeUserPlan = "users:plans:upgrade";

        // إذن تخفيض خطة المستخدم إلى خطة أقل
        public const string DowngradeUserPlan = "users:plans:downgrade";

        // إذن قراءة السيريال كودز الخاصة بمستخدم معين
        public const string GetSerialByUser = "users:serial:read";


        public const string GetRoles = "roles:read";
        public const string AddRoles = "roles:add";
        public const string UpdateRoles = "roles:update";

        public const string GetClaims = "claims:read";
        public const string AddClaims = "claims:add";
        public const string UpdateClaims = "claims:update";
        public const string DeleteClaims = "claims:delete";

        public const string GetUsers = "users:read";
        public const string AddUsers = "users:add";
        public const string UpdateUsers = "users:update";
        public const string DeleteUsers = "users:delete";
        public static IList<string?> GetAllPermissions() => typeof(Permissions).GetFields().Select(x => x.GetValue(x) as string).ToList();
    }
}
