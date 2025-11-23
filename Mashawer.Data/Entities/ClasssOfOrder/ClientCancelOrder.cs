namespace Mashawer.Data.Entities.ClasssOfOrder
{
    public class ClientCancelOrder
    {
        public int Id { get; set; }

        public string ClientId { get; set; }
        public ApplicationUser Client { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string CancelReason { get; set; }
    }
}
