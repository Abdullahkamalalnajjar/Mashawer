using Mashawer.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.Core.Features.Orders.Commands.Models
{
    public class UpdateOrderStatusCommand : IRequest<Response<string>>
    {
        public int OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }
}
