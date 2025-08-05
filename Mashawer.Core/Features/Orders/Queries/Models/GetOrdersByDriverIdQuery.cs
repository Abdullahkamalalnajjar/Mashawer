using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.Core.Features.Orders.Queries.Models
{
    public class GetOrdersByDriverIdQuery : IRequest<Response<IEnumerable<OrderDto>>>
    {
        public string? DriverId { get; set; } 
    }
}

