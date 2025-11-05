namespace Mashawer.Data.Dtos
{
    public class PurchaseItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceTotal { get; set; }
    }

}
