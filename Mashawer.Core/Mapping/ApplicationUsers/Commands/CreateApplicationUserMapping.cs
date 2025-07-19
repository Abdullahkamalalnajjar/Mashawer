namespace Mashawer.Core.Mapping.ApplicationUsers
{
    public partial class ApplicationUserProfile : Profile
    {
        public void CreateApplicationUserMapping()
        {
            CreateMap<SignUpUserCommand, ApplicationUser>()
              .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}
