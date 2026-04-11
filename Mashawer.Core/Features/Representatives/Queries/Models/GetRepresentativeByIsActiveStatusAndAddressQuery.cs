using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.Core.Features.Representatives.Queries.Models
{
    public class GetRepresentativeByIsActiveStatusAndAddressQuery : IRequest<Response<IEnumerable<RepresentativeDTO>>>
    {
        public GetRepresentativeByIsActiveStatusAndAddressQuery(string address, bool isActive)
        {
            Address = address;
            IsActive = isActive;
        }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
    
}
