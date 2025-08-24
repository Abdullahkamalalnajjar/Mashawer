using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.Core.Features.Orders.Queries.Models
{
    public class GetNearbyPendingOrdersQuery : IRequest<Response<IEnumerable<OrderDto>>>
    {
        public double DriverLatitude { get; set; }
        public double DriverLongitude { get; set; }
        public double RadiusKm { get; set; } = 20;   // الحد الأقصى 20 كم
    }

}
