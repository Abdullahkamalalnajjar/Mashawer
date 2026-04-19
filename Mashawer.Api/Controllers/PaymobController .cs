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
        private readonly ILogger<PaymobController> _logger;

        public PaymobController(PaymobService paymob, ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ILogger<PaymobController> logger)
        {
            _paymob = paymob;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _logger = logger;
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
            var userId = _currentUserService.UserId;
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
            _logger.LogInformation("Paymob webhook received: {Payload}", JsonSerializer.Serialize(payload));

            if (payload.Type == "TRANSACTION" && payload.Obj.Success)
            {
                _logger.LogInformation("Webhook is a successful transaction.");
                var merchantOrderId = payload.Obj.Order.MerchantOrderId;
                var amount = payload.Obj.AmountCents / 100m;
                _logger.LogInformation("Parsed MerchantOrderId: {MerchantOrderId}, Amount: {Amount}", merchantOrderId, amount);

                // 1) إذا كان هناك WalletTransaction مرتبط بالـ merchantOrderId حدّثه وزوّد رصيد المحفظة
                var walletTransaction = await _unitOfWork.WalletTransactions.GetTableAsTracking()
                    .FirstOrDefaultAsync(p => p.MerchantOrderId == merchantOrderId);

                if (walletTransaction != null && walletTransaction.Status != "Paid")
                {
                    _logger.LogInformation("Found matching WalletTransaction with ID: {TransactionId}", walletTransaction.Id);
                    walletTransaction.Status = "Paid";
                    walletTransaction.PaidAt = DateTime.UtcNow;

                    if (walletTransaction.Type == "Deposit")
                    {
                        var wallet = await _unitOfWork.Wallets.GetTableAsTracking()
                            .FirstOrDefaultAsync(w => w.Id == walletTransaction.WalletId);

                        if (wallet != null)
                        {
                            _logger.LogInformation("Updating wallet balance for wallet ID: {WalletId}. Old Balance: {OldBalance}, Amount: {Amount}", wallet.Id, wallet.Balance, amount);
                            wallet.Balance += amount;
                        }
                    }

                    await _unitOfWork.CompeleteAsync();
                    _logger.LogInformation("WalletTransaction updated successfully.");
                }

                // 2) إذا كان merchantOrderId يمثل رقم الطلب في التطبيق (app order id)
                if (int.TryParse(merchantOrderId, out var appOrderId))
                {
                    _logger.LogInformation("Successfully parsed MerchantOrderId as AppOrderId: {AppOrderId}", appOrderId);
                    var order = await _unitOfWork.Orders.GetTableAsTracking()
                        .FirstOrDefaultAsync(o => o.Id == appOrderId);

                    if (order != null)
                    {
                        _logger.LogInformation("Found matching order with ID: {OrderId}", order.Id);
                        var orderPrice = order.FinalPrice ?? 0m;

                        // تأكد من أن المبلغ المدفوع يطابق سعر الطلب
                        if (amount != orderPrice)
                        {
                            _logger.LogWarning("Paid amount ({PaidAmount}) does not match order final price ({OrderPrice}) for Order ID: {OrderId}", amount, orderPrice, order.Id);
                            return BadRequest("Paid amount does not match order total price.");
                        }

                        // ضع حالة الدفع واحتفظ بمعرّف الـ transaction
                        order.PaymentStatus = Mashawer.Data.Enums.PaymentStatus.Paid;
                        order.PaymobTransactionId = payload.Obj.Order.Id.ToString();
                        _logger.LogInformation("Updating order {OrderId} status to Paid.", order.Id);

                        // إذا كان للطلب مندوب، اخصم 25% من رصيد المندوب
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
                                // 1. Create and save the new wallet
                                var newWallet = new Mashawer.Data.Entities.Wallet
                                {
                                    UserId = order.DriverId,
                                    Balance = -commission
                                };
                                await _unitOfWork.Wallets.AddAsync(newWallet);
                                await _unitOfWork.CompeleteAsync(); // Save wallet to get its ID

                                // 2. Create the transaction with the new WalletId
                                var driverTx = new WalletTransaction
                                {
                                    WalletId = newWallet.Id, // Use the ID from the saved wallet
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
                    else
                    {
                        _logger.LogWarning("Order not found for AppOrderId: {AppOrderId}", appOrderId);
                    }
                }
                else
                {
                     _logger.LogWarning("Could not parse MerchantOrderId '{MerchantOrderId}' as an integer.", merchantOrderId);
                }
            }
            else
            {
                _logger.LogWarning("Webhook received but was not a successful transaction. Type: {Type}, Success: {Success}", payload.Type, payload.Obj.Success);
            }

            return Ok(); // لازم ترجع 200 عشان Paymob يعتبر الـ Webhook ناجح
        }



    }
}
