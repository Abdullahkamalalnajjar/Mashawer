using Mashawer.Data.Enums;
using Microsoft.AspNetCore.Identity;

namespace Mashawer.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public bool IsDisable { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public string? ProfilePictureUrl { get; set; }
        public UserType UserType { get; set; } = UserType.NormalUser;
        public string? Address { get; set; }
        public string? AgentAddress { get; set; }
        public string? RepresentativeAddress { get; set; }
        public double? RepresentativeFromLatitude { get; set; } // خط العرض لموقع الممثل    
        public double? RepresentativeToLatitude { get; set; } // خط العرض لموقع الممثل    
        public double? RepresentativeFromLongitude { get; set; } // خط الطول لموقع الممثل
        public double? RepresentativeToLongitude { get; set; } // خط الطول لموقع الممثل
        public string? VehicleType { get; set; }
        public string? VehicleNumber { get; set; }
        public string? VehiclePictureUrl { get; set; }
        public string? VehicleColor { get; set; }
        public string? FCMToken { get; set; }
        public bool IsProfileCompleted { get; set; } = false;

        public List<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
