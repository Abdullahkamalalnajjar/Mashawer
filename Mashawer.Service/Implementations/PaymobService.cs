using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

public class PaymobService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _unitOfWork;

    public PaymobService(HttpClient http, IConfiguration config, IUnitOfWork unitOfWork)
    {
        _http = http;
        _config = config;
        _unitOfWork = unitOfWork;
    }

    private string BaseUrl => _config["Paymob:ApiBaseUrl"]!.TrimEnd('/');
    private string ApiKey => _config["Paymob:ApiKey"]!;
    private string CardIntegrationId => _config["Paymob:CardIntegrationId"]!;
    private string WalletIntegrationId => _config["Paymob:WalletIntegrationId"]!;
    private string IFrameId => _config["Paymob:IFrameId"]!;
    private string HmacSecret => _config["Paymob:HmacSecret"]!;

    // 1) AUTH
    public async Task<string> AuthenticateAsync()
    {
        var res = await _http.PostAsJsonAsync($"{BaseUrl}/auth/tokens", new { api_key = ApiKey });
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadFromJsonAsync<AuthResponse>();
        return json!.token;
    }

    private record AuthResponse(string token);

    // 2) ORDER
    public async Task<long> CreateOrderAsync(string authToken, int amountCents, string currency, string merchantOrderId)
    {
        var body = new
        {
            auth_token = authToken,
            delivery_needed = false,
            amount_cents = amountCents,
            currency,
            merchant_order_id = merchantOrderId,
            items = Array.Empty<object>()
        };

        var res = await _http.PostAsJsonAsync($"{BaseUrl}/ecommerce/orders", body);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadFromJsonAsync<CreateOrderResponse>();
        return json!.id;
    }

    private record CreateOrderResponse(long id);

    // 3) PAYMENT KEY (generic)
    public async Task<string> CreatePaymentKeyAsync(
        string authToken, long orderId, int amountCents, string currency,
        PaymobBillingData billing, string integrationId)
    {
        var req = new
        {
            auth_token = authToken,
            amount_cents = amountCents.ToString(),
            expiration = 3600,
            order_id = orderId,
            currency,
            integration_id = integrationId,
            billing_data = new
            {
                first_name = billing.FirstName,
                last_name = billing.LastName,
                email = billing.Email,
                phone_number = billing.PhoneNumber,
                city = billing.City,
                country = billing.Country,
                state = billing.State,
                apartment = billing.Apartment,
                floor = billing.Floor,
                street = billing.Street,
                building = billing.Building,
                shipping_method = billing.ShippingMethod,
                postal_code = billing.PostalCode
            }
        };

        var res = await _http.PostAsJsonAsync($"{BaseUrl}/acceptance/payment_keys", req);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadFromJsonAsync<PaymentKeyResponse>();
        return json!.token;
    }


    private record PaymentKeyResponse(string token);

    // 4A) CARD FLOW: إرجاع IFrame URL
    public async Task<CardInitResponse> InitCardPaymentAsync(CreateCardPaymentRequest input, int walletId, CancellationToken cancellationToken = default)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var auth = await AuthenticateAsync();
            var merchantOrderId = string.IsNullOrWhiteSpace(input.MerchantOrderId)
                ? Guid.NewGuid().ToString("N")
                : input.MerchantOrderId;

            var orderId = await CreateOrderAsync(auth, input.AmountCents, input.Currency, merchantOrderId);
            var payToken = await CreatePaymentKeyAsync(auth, orderId, input.AmountCents, input.Currency, input.Billing, CardIntegrationId);
            var iframeUrl = $"{BaseUrl}/acceptance/iframes/{IFrameId}?payment_token={payToken}";
            var walletTransaction = new WalletTransaction
            {
                WalletId = walletId,
                MerchantOrderId = merchantOrderId,
                OrderId = orderId,
                Amount = input.AmountCents,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.WalletTransactions.AddAsync(walletTransaction, cancellationToken);
            await _unitOfWork.CompeleteAsync();
            await transaction.CommitAsync(cancellationToken);
            return new CardInitResponse { OrderId = orderId, PaymentToken = payToken, IframeUrl = iframeUrl };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    // 4B) WALLET FLOW: إرجاع Redirect URL
    public async Task<WalletInitResponse> InitWalletPaymentAsync(CreateWalletPaymentRequest input)
    {
        var auth = await AuthenticateAsync();
        var merchantOrderId = string.IsNullOrWhiteSpace(input.MerchantOrderId)
            ? Guid.NewGuid().ToString("N")
            : input.MerchantOrderId;

        var orderId = await CreateOrderAsync(auth, input.AmountCents, input.Currency, merchantOrderId);
        var payToken = await CreatePaymentKeyAsync(auth, orderId, input.AmountCents, input.Currency, input.Billing, WalletIntegrationId);

        // initiate wallet payment
        var payBody = new
        {
            source = new { identifier = input.WalletMsisdn, subtype = "WALLET" },
            payment_token = payToken
        };
        var res = await _http.PostAsJsonAsync($"{BaseUrl}/acceptance/payments/pay", payBody);
        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadFromJsonAsync<WalletPayResponse>();

        // بيكون فيه redirect_url (توجّه العميل عليه)
        return new WalletInitResponse { OrderId = orderId, PaymentToken = payToken, RedirectUrl = json!.redirect_url };
    }

    private record WalletPayResponse(string redirect_url);

    // (اختياري) التحقق من HMAC (الـ Webhook/Return URL)
    public bool VerifyHmac(IDictionary<string, string?> received, string providedHmac, string[] fieldOrder)
    {
        // Paymob بتحدد order لحقول معينة في الدوكيومنتشن—مرّرها في fieldOrder
        var sb = new StringBuilder();
        foreach (var key in fieldOrder)
            sb.Append(received.TryGetValue(key, out var v) ? v : string.Empty);

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(HmacSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
        var calc = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        return string.Equals(calc, providedHmac, StringComparison.OrdinalIgnoreCase);
    }
}
