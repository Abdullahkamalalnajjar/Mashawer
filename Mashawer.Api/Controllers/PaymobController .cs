using Mashawer.Api.Base;
using Mashawer.Data.Interfaces;
using Mashawer.Service.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Mashawer.Data.Dtos.PaymobDto;
using Mashawer.Data.Entities;

namespace Mashawer.Api.Controllers
{
    public class PaymobController : AppBaseController
    {
        private readonly PaymobService _paymob;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymobController> _logger;
        private readonly string _walletIntegrationId;

        public PaymobController(
            PaymobService paymob,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            ILogger<PaymobController> logger,
            IConfiguration configuration)
        {
            _paymob = paymob;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _walletIntegrationId = configuration["Paymob:WalletIntegrationId"] ?? string.Empty;
        }

        // ✅ (اختياري) فحص سريع للـ Auth
        [HttpGet("auth")]
        public async Task<IActionResult> Auth()
        {
            var token = await _paymob.AuthenticateAsync();
            return Ok(new { token });
        }

        // ✅ 1) إنشاء دفع بالكارت (Visa/Mastercard) — يرجّع IFrame URL
        [Authorize]
        [HttpPost("card/{amountCents}")]
        public async Task<IActionResult> CreateCardFromRoute([FromRoute] int amountCents)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) return Unauthorized("User not authenticated");

            var user = await _unitOfWork.Users.GetTableNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound("User not found.");

            var wallet = await _unitOfWork.Wallets.GetTableNoTracking().FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                var newWallet = new Mashawer.Data.Entities.Wallet
                {
                    UserId = userId,
                    Balance = 0
                };
                await _unitOfWork.Wallets.AddAsync(newWallet);
                await _unitOfWork.CompeleteAsync();

                wallet = await _unitOfWork.Wallets.GetTableNoTracking().FirstOrDefaultAsync(w => w.UserId == userId);
            }

            var req = new CreateCardPaymentRequest
            {
                AmountCents = amountCents,
            };

            var result = await _paymob.InitCardPaymentAsync(req);
            return Ok(result);
        }

        // ✅ 2) إنشاء دفع بالمحفظة — يرجّع Redirect URL
        [Authorize]
        [HttpPost("wallet/recharge")]
        public async Task<IActionResult> RechargeWalletByLocalWallet([FromBody] RechargeWalletByLocalWalletRequest req)
        {
            var userId = string.IsNullOrWhiteSpace(req.UserId)
                ? _currentUserService.UserId
                : req.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("User not authenticated");

            var user = await _unitOfWork.Users.GetTableNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound("User not found.");

            var result = await _paymob.InitWalletPaymentAsync(new CreateWalletPaymentRequest
            {
                AmountCents = req.AmountCents,
                WalletMsisdn = req.WalletMsisdn,
                Currency = "EGP",
                UserId = userId,
                Billing = new PaymobBillingData
                {
                    FirstName = user.FirstName ?? "User",
                    LastName = user.LastName ?? "Name",
                    Email = user.Email ?? "test@example.com",
                    PhoneNumber = user.PhoneNumber ?? req.WalletMsisdn
                }
            });

            return Ok(result);
        }

        [HttpPost("wallet")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletPaymentRequest req)
        {
            var result = await _paymob.InitWalletPaymentAsync(req);
            return Ok(result);
        }
        [HttpPost("webhook")]
        public Task<IActionResult> Webhook() => HandlePaymobWebhookAsync();

        [HttpPost("wallet/webhook")]
        public Task<IActionResult> WalletWebhook() => HandlePaymobWebhookAsync(expectedIntegrationId: _walletIntegrationId);

        [HttpPost("api/v1/paymob/wallet/webhook")]
        public Task<IActionResult> PaymobWalletWebhook() => HandlePaymobWebhookAsync(expectedIntegrationId: _walletIntegrationId);

        private async Task<(Dictionary<string, string?> Data, string ProvidedHmac)> ReadPaymobWebhookRequestAsync()
        {
            Request.EnableBuffering();

            Dictionary<string, string?> dict;
            string providedHmac;

            if (Request.ContentType != null &&
                Request.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;

                using var doc = JsonDocument.Parse(body);
                dict = FlattenJson(doc.RootElement);
                NormalizePaymobFields(dict);
                providedHmac = dict.TryGetValue("hmac", out var h) ? h ?? string.Empty : string.Empty;
            }
            else
            {
                var form = await Request.ReadFormAsync();
                dict = form.ToDictionary(k => k.Key, v => (string?)v.Value.ToString());
                providedHmac = dict.TryGetValue("hmac", out var h) ? h ?? string.Empty : string.Empty;
            }

            return (dict, providedHmac);
        }

        // دالة لفرد الـ JSON paths
        private Dictionary<string, string?> FlattenJson(JsonElement element, string? prefix = null)
        {
            var dict = new Dictionary<string, string?>();

            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var prop in element.EnumerateObject())
                    {
                        var key = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}.{prop.Name}";
                        foreach (var kv in FlattenJson(prop.Value, key))
                            dict[kv.Key] = kv.Value;
                    }
                    break;
                case JsonValueKind.Array:
                    int index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        var key = $"{prefix}[{index}]";
                        foreach (var kv in FlattenJson(item, key))
                            dict[kv.Key] = kv.Value;
                        index++;
                    }
                    break;
                default:
                    dict[prefix ?? ""] = element.ToString();
                    break;
            }

            return dict;
        }

        private static void NormalizePaymobFields(IDictionary<string, string?> dict)
        {
            var prefixedKeys = dict.Keys
                .Where(k => k.StartsWith("obj.", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var key in prefixedKeys)
            {
                var normalizedKey = key[4..];
                if (!dict.ContainsKey(normalizedKey))
                    dict[normalizedKey] = dict[key];
            }
        }

        private static string? GetPaymobValue(IReadOnlyDictionary<string, string?> dict, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (dict.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
                    return value;
            }

            return null;
        }

        private static bool IsTruthy(string? value) =>
            string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

        // ✅ 4) (اختياري) Return URL بعد الكارد (لو حابب ترجّع المستخدم لصفحة شكراً)
        [HttpGet("return")]
        public IActionResult Return([FromQuery] Dictionary<string, string> qs)
        {
            // تقدر تتحقق من hmac بنفس الطريقة (لكن بترتيب حقول return-url)
            // ثم تعمل Redirect لصفحة الـ Frontend بتاعتك مع النتيجة.
            var isSuccess = qs.TryGetValue("success", out var s) && s == "true";
            return Ok(new { success = isSuccess, query = qs });
        }
        [HttpGet("last-transaction/{orderId}")]
        public async Task<IActionResult> GetLastTransaction(int orderId)
        {
            var transactions = await _paymob.GetTransactionAsync(orderId);
            if (transactions == null)
            {
                return NotFound("Transactions not found or invalid format.");
            }
            return Ok(transactions);
        }

        public class PaymobWebhookDto
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("obj")]
            public PaymobObj Obj { get; set; }
        }

        public class PaymobObj
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("amount_cents")]
            public long AmountCents { get; set; }

            [JsonPropertyName("order")]
            public PaymobOrder Order { get; set; }
        }

        public class PaymobOrder
        {
            [JsonPropertyName("id")]
            public long Id { get; set; }

            [JsonPropertyName("merchant_order_id")]
            public string MerchantOrderId { get; set; }
        }



        [HttpPost("api/v1/paymob/webhook")]
        public Task<IActionResult> PaymobWebhook() => HandlePaymobWebhookAsync();

        private async Task<IActionResult> HandlePaymobWebhookAsync(string? expectedIntegrationId = null)
        {
            var (dict, providedHmac) = await ReadPaymobWebhookRequestAsync();
            _logger.LogInformation("Paymob webhook received: {Payload}", JsonSerializer.Serialize(dict));

            var order = new[]
            {
                "amount_cents","created_at","currency","error_occured","has_parent_transaction","id",
                "integration_id","is_3d_secure","is_auth","is_capture","is_refunded","is_standalone_payment",
                "is_voided","order.id","owner","pending","source_data.pan","source_data.sub_type",
                "source_data.type","success"
            };

            var ok = _paymob.VerifyHmac(dict, providedHmac, order);
            if (!ok)
                return Unauthorized("Invalid HMAC");

            var type = GetPaymobValue(dict, "type");
            var isSuccess = IsTruthy(GetPaymobValue(dict, "success", "obj.success"));
            var merchantOrderId = GetPaymobValue(dict, "order.merchant_order_id", "obj.order.merchant_order_id");
            var transactionId = GetPaymobValue(dict, "id", "obj.id");
            var amountCentsRaw = GetPaymobValue(dict, "amount_cents", "obj.amount_cents");
            var integrationId = GetPaymobValue(dict, "integration_id", "obj.integration_id");

            if (!string.IsNullOrWhiteSpace(expectedIntegrationId) &&
                !string.IsNullOrWhiteSpace(integrationId) &&
                !string.Equals(integrationId, expectedIntegrationId, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "Ignoring Paymob webhook because integration id {IntegrationId} does not match expected integration id {ExpectedIntegrationId}.",
                    integrationId,
                    expectedIntegrationId);
                return Ok();
            }

            if (isSuccess &&
                decimal.TryParse(amountCentsRaw, out var amountCents))
            {
                _logger.LogInformation("Webhook is a successful transaction.");
                _logger.LogInformation("Parsed MerchantOrderId: {MerchantOrderId}, Amount: {Amount}", merchantOrderId, amountCents / 100m);
                await ProcessSuccessfulTransactionAsync(merchantOrderId, amountCents / 100m, transactionId);
            }
            else
            {
                _logger.LogWarning("Webhook received but was not a successful transaction. Type: {Type}, Success: {Success}", type, isSuccess);

                if (int.TryParse(merchantOrderId, out var failedOrderId))
                {
                    var failedOrder = await _unitOfWork.Orders.GetTableAsTracking()
                        .FirstOrDefaultAsync(o => o.Id == failedOrderId);

                    if (failedOrder != null &&
                        failedOrder.PaymentMethod == Mashawer.Data.Enums.PaymentMethod.Visa &&
                        failedOrder.PaymentStatus != Mashawer.Data.Enums.PaymentStatus.Paid)
                    {
                        _logger.LogInformation("Cancelling unpaid Visa order {OrderId} after failed payment.", failedOrder.Id);
                        failedOrder.Status = Mashawer.Data.Enums.OrderStatus.Cancelled;
                        failedOrder.PaymentStatus = Mashawer.Data.Enums.PaymentStatus.Failed;
                        await _unitOfWork.CompeleteAsync();
                    }
                }
            }

            return Ok(); // لازم ترجع 200 عشان Paymob يعتبر الـ Webhook ناجح
        }

        private async Task ProcessSuccessfulTransactionAsync(string? merchantOrderId, decimal amount, string? paymobTransactionId)
        {
            if (string.IsNullOrWhiteSpace(merchantOrderId))
            {
                _logger.LogWarning("Skipping successful Paymob transaction because MerchantOrderId is missing.");
                return;
            }

            var walletTransaction = await _unitOfWork.WalletTransactions.GetTableAsTracking()
                .FirstOrDefaultAsync(p => p.MerchantOrderId == merchantOrderId);

            if (walletTransaction != null && walletTransaction.Status != "Paid")
            {
                _logger.LogInformation("Found matching WalletTransaction with ID: {TransactionId}", walletTransaction.Id);
                walletTransaction.Status = "Paid";
                walletTransaction.PaidAt = DateTime.UtcNow;

                if (string.Equals(walletTransaction.Type, "Deposit", StringComparison.OrdinalIgnoreCase))
                {
                    var wallet = await _unitOfWork.Wallets.GetTableAsTracking()
                        .FirstOrDefaultAsync(w => w.Id == walletTransaction.WalletId);

                    if (wallet != null)
                    {
                        _logger.LogInformation(
                            "Updating wallet balance for wallet ID: {WalletId}. Old Balance: {OldBalance}, Amount: {Amount}",
                            wallet.Id, wallet.Balance, amount);
                        wallet.Balance += amount;
                    }
                }

                await _unitOfWork.CompeleteAsync();
                _logger.LogInformation("WalletTransaction updated successfully.");
            }

            if (!int.TryParse(merchantOrderId, out var appOrderId))
            {
                _logger.LogInformation("MerchantOrderId '{MerchantOrderId}' is not an app order id. Skipping order update.", merchantOrderId);
                return;
            }

            _logger.LogInformation("Successfully parsed MerchantOrderId as AppOrderId: {AppOrderId}", appOrderId);
            var order = await _unitOfWork.Orders.GetTableAsTracking()
                .FirstOrDefaultAsync(o => o.Id == appOrderId);

            if (order == null)
            {
                _logger.LogWarning("Order not found for AppOrderId: {AppOrderId}", appOrderId);
                return;
            }

            _logger.LogInformation("Found matching order with ID: {OrderId}", order.Id);
            var orderPrice = order.FinalPrice ?? 0m;

            if (amount != orderPrice)
            {
                _logger.LogWarning(
                    "Paid amount ({PaidAmount}) does not match order final price ({OrderPrice}) for Order ID: {OrderId}",
                    amount, orderPrice, order.Id);
                return;
            }

            order.PaymentStatus = Mashawer.Data.Enums.PaymentStatus.Paid;
            order.Status = Mashawer.Data.Enums.OrderStatus.Pending;
            order.PaymobTransactionId = paymobTransactionId ?? order.PaymobTransactionId;
            _logger.LogInformation("Updating order {OrderId} status to Paid.", order.Id);

            if (!string.IsNullOrEmpty(order.DriverId))
            {
                _logger.LogInformation("Order {OrderId} has a driver. Calculating commission.", order.Id);
                var commission = orderPrice * 0.25m;

                var driverWallet = await _unitOfWork.Wallets.GetTableAsTracking()
                    .FirstOrDefaultAsync(w => w.UserId == order.DriverId);

                if (driverWallet != null)
                {
                    driverWallet.Balance -= commission;

                    var driverTx = new WalletTransaction
                    {
                        WalletId = driverWallet.Id,
                        Amount = commission,
                        Type = "Withdraw",
                        Status = "Paid",
                        PaidAt = DateTime.UtcNow,
                        OrderId = appOrderId
                    };
                    await _unitOfWork.WalletTransactions.AddAsync(driverTx);
                }
                else
                {
                    _logger.LogInformation("Driver {DriverId} does not have a wallet. Creating one.", order.DriverId);
                    var newWallet = new Mashawer.Data.Entities.Wallet
                    {
                        UserId = order.DriverId,
                        Balance = -commission
                    };
                    await _unitOfWork.Wallets.AddAsync(newWallet);
                    await _unitOfWork.CompeleteAsync();

                    var driverTx = new WalletTransaction
                    {
                        WalletId = newWallet.Id,
                        Amount = commission,
                        Type = "Withdraw",
                        Status = "Paid",
                        PaidAt = DateTime.UtcNow,
                        OrderId = appOrderId
                    };
                    await _unitOfWork.WalletTransactions.AddAsync(driverTx);
                }
            }

            await _unitOfWork.CompeleteAsync();
            _logger.LogInformation("Successfully updated order {OrderId} and related entities.", order.Id);
        }



    }
}
