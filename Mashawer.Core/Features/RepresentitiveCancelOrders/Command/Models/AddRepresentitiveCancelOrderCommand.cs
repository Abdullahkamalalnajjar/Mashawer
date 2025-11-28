namespace Mashawer.Core.Features.RepresentitiveCancelOrders.Command.Models
{
    public class AddRepresentitiveCancelOrderCommand : IRequest<Response<string>>
    {
        public int OrderId { get; set; }
        public string Reason { get; set; }
        public AddRepresentitiveCancelOrderCommand(int orderId, string reason)
        {
            OrderId = orderId;
            Reason = reason;
        }

    }

}
