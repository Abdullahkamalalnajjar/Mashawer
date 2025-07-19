namespace Mashawer.Core.Mapping.ApplicationUsers
{
    public partial class ApplicationUserProfile : Profile
    {
        public void EditApplicationUserMapping()
        {
            CreateMap<EditApplicationUserCommand, ApplicationUser>();
        }
    }
}
