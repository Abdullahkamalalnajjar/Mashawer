namespace Mashawer.Core.Features.ClientCancelOrders.Command.Model
{
    public class AddClientCancelOrderCommand : IRequest<Response<string>>
    {
        public int OrderId { get; set; }
        public string Reason { get; set; }

        public AddClientCancelOrderCommand(int orderId, string reason)
        {
            OrderId = orderId;
            Reason = reason;
        }
    }
}