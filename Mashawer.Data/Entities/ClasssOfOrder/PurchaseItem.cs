namespace Mashawer.Data.Entities.ClasssOfOrder
{
    public class PurchaseItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }
        public decimal PriceTotal => Quantity * Price;

        public int OrderTaskId { get; set; }           
        public OrderTask OrderTask { get; set; } = null!;

    }
}
