using Mashawer.Api.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Mashawer.Data.Dtos.PaymobDto;

namespace Mashawer.Api.Controllers
{
    public class PaymobController : AppBaseController
    {
        private readonly PaymobService _paymob;

        public PaymobController(PaymobService paymob) => _paymob = paymob;

        // ✅ (اختياري) فحص سريع للـ Auth
        [HttpGet("auth")]
        public async Task<IActionResult> Auth()
        {
            var token = await _paymob.AuthenticateAsync();
            return Ok(new { token });
        }

        // ✅ 1) إنشاء دفع بالكارت (Visa/Mastercard) — يرجّع IFrame URL
        [HttpPost("card")]
        public async Task<IActionResult> CreateCard([FromBody] CreateCardPaymentRequest req)
        {
            var result = await _paymob.InitCardPaymentAsync(req);
            return Ok(result);
        }

        // ✅ 2) إنشاء دفع بالمحفظة — يرجّع Redirect URL
        [HttpPost("wallet")]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletPaymentRequest req)
        {
            var result = await _paymob.InitWalletPaymentAsync(req);
            return Ok(result);
        }

        // ✅ 3) (اختياري) Webhook لاستلام حالة العملية من Paymob
        // اضبط الـ URL ده في لوحة Paymob.
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            // استقبل الـ payload كـ form أو JSON حسب إعداد Paymob
            // هنا مثال مبسّط في حالة form-urlencoded:
            Request.EnableBuffering();
            var form = await Request.ReadFormAsync();
            var dict = form.ToDictionary(k => k.Key, v => v.Value.ToString());

            var providedHmac = dict.TryGetValue("hmac", out var h) ? h : "";
            // ⚠️ IMPORTANT: حدد ترتيب الحقول طبقًا لـ Paymob docs (مثال شائع للـ transaction callback):
            var order = new[]
            {
            "amount_cents","created_at","currency","error_occured","has_parent_transaction","id",
            "integration_id","is_3d_secure","is_auth","is_capture","is_refunded","is_standalone_payment",
            "is_voided","order.id","owner","pending","source_data.pan","source_data.sub_type",
            "source_data.type","success"
        };

            var ok = _paymob.VerifyHmac(dict, providedHmac, order);
            if (!ok) return Unauthorized("Invalid HMAC");

            // TODO: حدّث حالة الطلب في DB عندك (Paid/Failed...) باستخدام order.id أو merchant_order_id
            return Ok();
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
    }
}
