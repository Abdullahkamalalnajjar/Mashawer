using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.Orders.Commands.Models
{
    public class UpdateOrderStatusCommand : IRequest<Response<string>>
    {
        public int OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
        public string? DriverId { get; set; }
    }
}
