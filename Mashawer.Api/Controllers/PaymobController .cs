using Mashawer.Api.Base;
using Mashawer.Data.Interfaces;
using Mashawer.Service.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly HttpClient _http = new HttpClient();
        public PaymobController(PaymobService paymob, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
        {
            _paymob = paymob;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
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
        [HttpPost("card")]
        public async Task<IActionResult> CreateCard([FromBody] CreateCardPaymentRequest req)
        {
            var userId = _currentUserService.UserId;
            if (userId == null) return Unauthorized("User not authenticated");
            var walletId = await _unitOfWork.Wallets.GetTableNoTracking().Where(w => w.UserId == userId).Select(w => w.Id).FirstOrDefaultAsync();
            var result = await _paymob.InitCardPaymentAsync(req, walletId);
            return Ok(result);
        }

        // ✅ 2) إنشاء دفع بالمحفظة — يرجّع Redirect URL
        [HttpPost("wallet")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletPaymentRequest req)
        {
            var result = await _paymob.InitWalletPaymentAsync(req);
            return Ok(result);
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            Request.EnableBuffering();
            string providedHmac;
            Dictionary<string, string?> dict;

            if (Request.ContentType != null &&
                Request.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                // قراءة JSON
                using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;

                using var doc = JsonDocument.Parse(body);
                dict = FlattenJson(doc.RootElement);

                providedHmac = dict.TryGetValue("hmac", out var h) ? h : "";
            }
            else
            {
                // قراءة form-urlencoded
                var form = await Request.ReadFormAsync();
                dict = form.ToDictionary(k => k.Key, v => v.Value.ToString());
                providedHmac = dict.TryGetValue("hmac", out var h) ? h : "";
            }

            // ترتيب الحقول حسب Documentation
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

            // تحديث الطلب في قاعدة البيانات
            return Ok();
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
            string apiKey = "ZXlKaGJHY2lPaUpJVXpVeE1pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SmpiR0Z6Y3lJNklrMWxjbU5vWVc1MElpd2ljSEp2Wm1sc1pWOXdheUk2TVRFd056VXdNaXdpYm1GdFpTSTZJbWx1YVhScFlXd2lmUS5xWlpOMDZlVUtTRGdTdTAtOTBrZ3Z5TUlSUXBZMDZBWHBBeHcyVjlVZGdyb0hYaG9kVG5tY0dUS3BaUHhubEhEOVdDSXJodUxaUExyMDJwUzFQSW9TZw=="; // API Key بتاعك

            // 1. Get Auth Token
            var authResponse = await _http.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens", new
            {
                api_key = apiKey
            });

            if (!authResponse.IsSuccessStatusCode)
                return BadRequest("Auth failed");

            var authJson = await authResponse.Content.ReadAsStringAsync();
            var authData = JsonSerializer.Deserialize<Dictionary<string, object>>(authJson);
            string token = authData?["token"]?.ToString() ?? "";

            // 2. Get Transactions by orderId
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var transResponse = await _http.GetAsync($"https://accept.paymob.com/api/acceptance/transactions?order_id={orderId}");

            if (!transResponse.IsSuccessStatusCode)
                return BadRequest("Failed to get transactions");

            var transJson = await transResponse.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(transJson);

            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                var transactions = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(transJson);
                return Ok(transactions);
            }
            else if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                if (doc.RootElement.TryGetProperty("transactions", out var txElement) && txElement.ValueKind == JsonValueKind.Array)
                {
                    var transactions = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(txElement.GetRawText());
                    return Ok(transactions);
                }
                return BadRequest("No transactions array found");
            }

            return BadRequest("Invalid response format");
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
        public async Task<IActionResult> PaymobWebhook([FromBody] PaymobWebhookDto payload)
        {
            if (payload.Type == "TRANSACTION" && payload.Obj.Success)
            {
                var merchantOrderId = payload.Obj.Order.MerchantOrderId;
                var amount = payload.Obj.AmountCents / 100m;

                // 1) إذا كان هناك WalletTransaction مرتبط بالـ merchantOrderId حدّثه وزوّد رصيد المحفظة
                var walletTransaction = await _unitOfWork.WalletTransactions.GetTableAsTracking()
                    .FirstOrDefaultAsync(p => p.MerchantOrderId == merchantOrderId);

                if (walletTransaction != null && walletTransaction.Status != "Paid")
                {
                    walletTransaction.Status = "Paid";
                    walletTransaction.PaidAt = DateTime.UtcNow;

                    var wallet = await _unitOfWork.Wallets.GetTableAsTracking()
                        .FirstOrDefaultAsync(w => w.Id == walletTransaction.WalletId);

                    if (wallet != null)
                    {
                        wallet.Balance += amount;
                    }

                    await _unitOfWork.CompeleteAsync();
                }

                // 2) إذا كان merchantOrderId يمثل رقم الطلب في التطبيق (app order id)
                if (int.TryParse(merchantOrderId, out var appOrderId))
                {
                    var order = await _unitOfWork.Orders.GetTableAsTracking()
                        .FirstOrDefaultAsync(o => o.Id == appOrderId);

                    if (order != null)
                    {
                        var orderPrice = order.TotalPrice ?? 0m;

                        // تأكد من أن المبلغ المدفوع يطابق سعر الطلب
                        if (amount != orderPrice)
                        {
                            // إذا لم يتطابق المبلغ، نُخطر ونوقف المتابعة (يمكن تغيير السلوك حسب الحاجة)
                            return BadRequest("Paid amount does not match order total price.");
                        }

                        // ضع حالة الدفع واحتفظ بمعرّف الـ transaction
                        order.PaymentStatus = Mashawer.Data.Enums.PaymentStatus.Paid;
                        order.PaymobTransactionId = payload.Obj.Order.Id.ToString();

                        // إذا كان للطلب مندوب، اخصم 25% من رصيد المندوب
                        if (!string.IsNullOrEmpty(order.DriverId))
                        {
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
                                // إنشاء محفظة جديدة بالمندوب مع رصيد سالب (اختياري)
                                var newWallet = new Mashawer.Data.Entities.Wallet
                                {
                                    UserId = order.DriverId,
                                    Balance = -commission
                                };
                                await _unitOfWork.Wallets.AddAsync(newWallet);
                                await _unitOfWork.CompeleteAsync();

                                var created = await _unitOfWork.Wallets.GetTableAsTracking()
                                    .FirstOrDefaultAsync(w => w.UserId == order.DriverId);
                                if (created != null)
                                {
                                    var driverTx = new WalletTransaction
                                    {
                                        WalletId = created.Id,
                                        Amount = commission,
                                        Type = "Withdraw",
                                        Status = "Paid",
                                        PaidAt = DateTime.UtcNow,
                                        OrderId = appOrderId
                                    };
                                    await _unitOfWork.WalletTransactions.AddAsync(driverTx);
                                }
                            }
                        }

                        await _unitOfWork.CompeleteAsync();
                    }
                }
            }

            return Ok(); // لازم ترجع 200 عشان Paymob يعتبر الـ Webhook ناجح
        }



    }
}

