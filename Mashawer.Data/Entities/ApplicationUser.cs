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
        public string? ProfilePictureUrl { get; set; }
        public UserType UserType { get; set; } = UserType.NormalUser;
        public string? Address { get; set; }
        public string? AgentAddress { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
