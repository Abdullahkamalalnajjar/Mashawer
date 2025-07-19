namespace Mashawer.Core.Mapping.UserUpgradeRequests
{
    public partial class UserUpgradeRequestProfile : Profile
    {
        public UserUpgradeRequestProfile()
        {
            CreateUserUpgradeCommand_Mapper();
            EditUserUpgradeCommand_Mapper();
        }
    }
}
