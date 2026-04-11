namespace Mashawer.Core.Features.Representatives.Command.Models
{
    public class UpdateRepresentativesLocationCommand : IRequest<Response<string>>
    {
        public string UserId { get; set; }
        public double RepresentativeFromLatitude { get; set; } // خط العرض لموقع الممثل    
        public double RepresentativeToLatitude { get; set; } // خط العرض لموقع الممثل    
        public double RepresentativeFromLongitude { get; set; } // خط الطول لموقع الممثل
        public double RepresentativeToLongitude { get; set; } // خط الطول لموقع الممثل

    }

}

