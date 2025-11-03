namespace Mashawer.Core.Features.Representatives.Command.Models
{
    public class MarkIsClientLateCommand : IRequest<Response<string>>
    {
        public int OrderId { get; set; }
        public MarkIsClientLateCommand(int orderId)
        {
            OrderId = orderId;
        }

    }
}
