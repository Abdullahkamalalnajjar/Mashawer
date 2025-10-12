namespace Mashawer.Data.Dtos
{
    public class PaymobDto
    {
        public class PaymobBillingData
        {
            public string FirstName { get; set; } = "User";
            public string LastName { get; set; } = "Name";
            public string Email { get; set; } = "test@example.com";
            public string PhoneNumber { get; set; } = "+201000000000";
            public string City { get; set; } = "Cairo";
            public string Country { get; set; } = "EG";
            public string State { get; set; } = "Cairo";
            // Paymob requires these fields to exist (even لو فاضية)
            public string Apartment { get; set; } = "NA";
            public string Floor { get; set; } = "NA";
            public string Street { get; set; } = "NA";
            public string Building { get; set; } = "NA";
            public string ShippingMethod { get; set; } = "NA";
            public string PostalCode { get; set; } = "NA";
        }

        public class CreateCardPaymentRequest
        {
            public int AmountCents { get; set; }        // مثال: 10000 = 100 EGP
            public string Currency { get; set; } = "EGP";
            public string? MerchantOrderId { get; set; } // لو سيبتها فاضية هنولد GUID
            public PaymobBillingData Billing { get; set; } = new();
        }

        public class CreateWalletPaymentRequest
        {
            public int AmountCents { get; set; }
            public string Currency { get; set; } = "EGP";
            public string WalletMsisdn { get; set; } = "01000000000"; // رقم المحفظة
            public string? MerchantOrderId { get; set; }
            public PaymobBillingData Billing { get; set; } = new();
        }

        public class CardInitResponse
        {
            public long OrderId { get; set; }
            public string PaymentToken { get; set; }
            public string IframeUrl { get; set; }
        }

        public class WalletInitResponse
        {
            public long OrderId { get; set; }
            public string PaymentToken { get; set; }
            public string RedirectUrl { get; set; } // تفتحه للعميل
        }
    }
}
