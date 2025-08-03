using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.UserUpgradeRequests.Command.Models
{
    public class CreateUserUpgradeRequestCommand : IRequest<Response<string>>
    {

        public string UserId { get; set; }

        public RequestedRole RequestedRole { get; set; }
        public string? Note { get; set; }
        public string? Address { get; set; }
        public string? TargetAgentId { get; set; }
        public double? RepresentativeLatitude { get; set; } // خط العرض لموقع الممثل
        public double? RepresentativeLongitude { get; set; } // خط الطول لموقع الممثل
    }
}
