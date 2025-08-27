namespace Mashawer.Data.Entities
{
    public class GeneralSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// نسبة الخصم على الأوردر (مثلاً 10% = 0.1)
        /// </summary>
        public decimal DiscountPercentage { get; set; }
    }
}
