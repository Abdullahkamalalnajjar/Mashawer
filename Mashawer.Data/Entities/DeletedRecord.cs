namespace Mashawer.Data.Entities
{
    public class DeletedRecord
    {
        public int Id { get; set; }
        public string Email { get; set; }        // الإيميل المرتبط بالكيان
        public string Reason { get; set; }       // سبب الحذف
        public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
    }

}
