using Mashawer.Api.Base;
using Mashawer.Data.Interfaces;
using Mashawer.Service.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static Mashawer.Data.Dtos.PaymobDto;

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
            string apiKey = "ZXlKaGJHY2lPaUpJVXpVeE1pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SmpiR0Z6Y3lJNklrMWxjbU5vWVc1MElpd2ljSEp2Wm1sc1pWOXdheUk2TVRBMk5UQXdNeXdpYm1GdFpTSTZJbWx1YVhScFlXd2lmUS5UQjBteVNBSlRsWFlmc3BCTEYzTUlCTHZVTWVoV3BjVGIxSE8ycE1MbTJjYVVXX0pDckxzWllpX1M5OENFQUZZcFJWS3dzVkRZSnBGb3h6SGJQSFRfQQ=="; // API Key بتاعك

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

        //[HttpPost("api/v1/paymob/webhook")]
        //public async Task<IActionResult> PaymobWebhook([FromBody] PaymobWebhookDto payload)
        //{
        //    if (payload.Type == "TRANSACTION" && payload.Obj.Success)
        //    {
        //        var merchantOrderId = payload.Obj.Order.MerchantOrderId;
        //        var amount = payload.Obj.AmountCents / 100m;

        //        // ✅ حدّث حالة الدفع
        //        var payment = await _context.Payments
        //            .FirstOrDefaultAsync(p => p.MerchantOrderId == merchantOrderId);

        //        if (payment != null && !payment.IsPaid)
        //        {
        //            payment.IsPaid = true;
        //            payment.UpdatedAt = DateTime.UtcNow;

        //            // ✅ زوّد رصيد المحفظة
        //            var wallet = await _context.Wallets
        //                .FirstOrDefaultAsync(w => w.CustomerId == payment.CustomerId);

        //            if (wallet == null)
        //            {
        //                wallet = new Wallet
        //                {
        //                    CustomerId = payment.CustomerId,
        //                    Balance = 0,
        //                    CreatedAt = DateTime.UtcNow
        //                };
        //                _context.Wallets.Add(wallet);
        //            }

        //            wallet.Balance += amount;

        //            // ✅ سجل العملية في WalletTransaction
        //            var transaction = new WalletTransaction
        //            {
        //                WalletId = wallet.Id,
        //                Amount = amount,
        //                Type = "Deposit",
        //                Description = $"Paymob payment for Order {merchantOrderId}",
        //                CreatedAt = DateTime.UtcNow
        //            };
        //            _context.WalletTransactions.Add(transaction);

        //            await _context.SaveChangesAsync();
        //        }
        //    }

        //    return Ok(); // لازم ترجع 200 عشان Paymob يعتبر الـ Webhook ناجح
        //}


    }
}

