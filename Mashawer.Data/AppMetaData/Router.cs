namespace Mashawer.Data.AppMetaData
{
    public static class Router
    {

        private const string root = "Api";
        private const string version = "V1";
        private const string Rule = root + "/" + version;
        private const string SingleRoute = "{id}";
        private const string ListRoute = "List";
        public static class GoogleRouting
        {
            private const string Prefix = Rule + "/" + "Google/";
            public const string GoogleSignIn = Prefix + "GoogleSignIn";
            public const string GoogleLoginCallback = Prefix + "GoogleLoginCallback";
        }

        public static class RepresentativeRouting
        {
            private const string Prefix = Rule + "/" + "Representative/";
            public const string List = Prefix + ListRoute;
            public const string GetById = Prefix + SingleRoute;
            public const string Create = Prefix + "Create";
            public const string Edit = Prefix + "Edit";
            public const string Delete = Prefix + "Delete/{id}";
            public const string Paginated = Prefix + "Paginated";
            public const string GetApprovedRepresentativesByAddress = Prefix + "GetApprovedRepresentativesByAddress/{address}";
            public const string NearestRepresentative = Prefix + "NearestRepresentative";
            public const string UpdateRepresentativeLivelocation = Prefix + "UpdateRepresentativeLivelocation";

        }
        public static class UserUpgradeRequestRouting
        {
            private const string Prefix = Rule + "/" + "UserUpgradeRequest/";
            public const string List = Prefix + ListRoute;
            public const string GetById = Prefix + SingleRoute;
            public const string GetByUserId = Prefix + "GetByUserId/{userId}";
            public const string GetByAgentId = Prefix + "GetByAgentId/{agentId}";
            public const string GetByAddress = Prefix + "GetByAddress/{address}";
            public const string FilterByTypeOrAddress = Prefix + "FilterByTypeOrAddress";
            public const string Create = Prefix + "Create";
            public const string Edit = Prefix + "Edit";
            public const string Delete = Prefix + "Delete/{id}";
            public const string Paginated = Prefix + "Paginated";
            public const string ApproveRequest = Prefix + "ApproveRequest/{id}";
            public const string RejectRequest = Prefix + "RejectRequest/{id}";
        }
        public static class OrderRouting
        {
            private const string Prefix = Rule + "/" + "Order/";
            public const string List = Prefix + ListRoute;
            public const string GetById = Prefix + SingleRoute;
            public const string Create = Prefix + "Create";
            public const string Edit = Prefix + "Edit";
            public const string UpdateStatus = Prefix + "UpdateStatus";
            public const string Delete = Prefix + "Delete/{id}";
            public const string Paginated = Prefix + "Paginated";
            public const string GetOrdersByClientId = Prefix + "GetOrdersByClientId";
            public const string GetOrdersByDriverId = Prefix + "GetOrdersByDriverId";
            public const string GetOrdersForUser = Prefix + "GetOrdersForUser";
            public const string GetNearbyPendingOrders = Prefix + "GetNearbyPendingOrders";
            public const string UploadOrderPhotos = "Api/V1/Order/UploadPhotos";
        }
        public static class PaymentRouting
        {
            private const string Prefix = Rule + "/" + "Payment/";
            public const string CreatePayment = Prefix + "CreatePayment";
            public const string GetPaymentById = Prefix + SingleRoute;
            public const string GetPaymentsForUser = Prefix + "GetPaymentsForUser";
            public const string GetPaymentIntent = Prefix + "GetPaymentIntent/{basketId}";
            public const string UpdatePaymentStatus = Prefix + "UpdatePaymentStatus/{paymentId}";
        }
        public static class SerialCodeRouting
        {
            private const string Prefix = Rule + "/" + "SerialCode/";
            public const string GenerateSerialCode = Prefix + "GenerateSerialCode";
            public const string GetSerialCodes = Prefix + ListRoute;
            public const string GetSerialCodeById = Prefix + SingleRoute;
            public const string GetSerialCodesForUser = Prefix + "GetSerialCodesForUser";
            public const string ActivateSerialCode = Prefix + "ActivateSerialCode/{serialCode}";
            public const string ValidateSerialCode = Prefix + "ValidateSerialCode/{serialCode}";
        }
        public static class EmailRouting
        {
            private const string Prefix = Rule + "/" + "Email/";
            public const string SendEmail = Prefix + "SendEmail";

        }
        public static class UserRouting
        {
            private const string Prefix = Rule + "/" + "AppUser/";
            public const string List = Prefix + ListRoute;
            public const string GetById = Prefix + SingleRoute;
            public const string Create = Prefix + "Create";
            public const string Edit = Prefix + "Edit";
            public const string Delete = Prefix + "Delete/{id}";
            public const string Paginated = Prefix + "Paginated";
            public const string ChangePassword = Prefix + "ChangePassword";
        }
        public static class RoleRouting
        {
            private const string Prefix = Rule + "/" + "Role/";
            public const string List = Prefix + ListRoute;
            public const string GetById = Prefix + SingleRoute;
            public const string Create = Prefix + "Create";
            public const string Edit = Prefix + "Edit";
            public const string Delete = Prefix + "Delete/{id}";
            public const string Paginated = Prefix + "Paginated";
        }
        public static class AuthenticationRouting
        {
            private const string Prefix = Rule + "/" + "Authentication/";
            public const string SginIn = Prefix + "SginIn";
            public const string SignUp = Prefix + "SignUp";
            public const string RefreshToken = Prefix + "RefreshToken";
            public const string RevokeRefreshToken = Prefix + "RevokeRefreshToken";
            public const string ChangePassword = Prefix + "ChangePassword";
            public const string SendOtp = Prefix + "SendOtp";
            public const string Verifyotp = Prefix + "verify-otp";
            public const string ResetPassword = Prefix + "reset-password";
            public const string ConfirmEmail = Prefix + "ConfirmEmail";
            public const string ResendConfirmEmail = Prefix + "ResendConfirmEmail";
        }
        public static class AuthorizationRouting
        {
            private const string Prefix = Rule + "/" + "Authorization/";
            public const string CreateRole = Prefix + "CreateRole";
            public const string MangeUserRoles = Prefix + "mange-user-roles/{userId}";
            public const string GetRolesList = Prefix + "GetRolesList";
            public const string GetClimsList = Prefix + "GetClaimsList";
            public const string MangeUserClaims = Prefix + "mange-user-calims/{userId}";
            public const string UpdateRole = Prefix + "UpdateRole";
            public const string UpdateClaims = Prefix + "UpdateClaims";

        }
        public static class GeneralSettingRouting
        {
            private const string Prefix = Rule + "/" + "GeneralSetting/";
            public const string GetGeneralSetting = Prefix + "GetGeneralSetting";
            public const string Create = Prefix + "Create";
            public const string Edit = Prefix + "Edit";

        }
    }
}
