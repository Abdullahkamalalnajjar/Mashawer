namespace Mashawer.Core.Mapping.ApplicationUsers
{
    public partial class ApplicationUserProfile
    {
        public void UserProfileQueryMapper()
        {
            CreateMap<ApplicationUser, UserProfileResponse>();
        }
    }
}
