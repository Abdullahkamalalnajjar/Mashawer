namespace Mashawer.Data.Dtos
{
    public class PaymobWebhookDto
    {
        public string Id { get; set; }
        public bool Success { get; set; }
        public string Type { get; set; }
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public string Hmac { get; set; }
        public DateTime CreatedAt { get; set; }
        public PaymobWebhookData Data { get; set; }
    }

    public class PaymobWebhookData
    {
        public string Id { get; set; }
        public string IntegrationId { get; set; }
        public bool Success { get; set; }
        public string Currency { get; set; }
        public decimal AmountCents { get; set; }
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public string Hmac { get; set; }
    }

}
