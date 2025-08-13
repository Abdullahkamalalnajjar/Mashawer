using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mashawer.Core.Features.Representatives.Command.Models
{
    public class UpdateRepresentativesLocationCommand : IRequest<Response<string>>
    {
        public string UserId { get; set; }
        public double RepresentativeLatitude { get; set; } // خط العرض لموقع الممثل    
        public double RepresentativeLongitude { get; set; } // خط الطول لموقع الممثل

    }
    
    }

