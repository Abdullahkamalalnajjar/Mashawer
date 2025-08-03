using Mashawer.Core.Features.Orders.Commands.Models;
using Mashawer.Data.Entities.ClasssOfOrder;

namespace Mashawer.Core.Mapping.Orders
{
    public partial class OrderProfile
    {
        public void CreateOrder_Mapping()
        {
            CreateMap<CreateOrderCommand, Order>();
        }
    }
}
