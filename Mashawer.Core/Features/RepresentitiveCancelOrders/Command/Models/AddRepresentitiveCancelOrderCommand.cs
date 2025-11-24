using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
