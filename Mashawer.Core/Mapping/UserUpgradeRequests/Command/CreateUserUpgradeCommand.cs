using Mashawer.Core.Features.UserUpgradeRequests.Command.Models;

namespace Mashawer.Core.Mapping.UserUpgradeRequests
{
    public partial class UserUpgradeRequestProfile
    {
        public void CreateUserUpgradeCommand_Mapper()
        {
            CreateMap<CreateUserUpgradeRequestCommand, UserUpgradeRequest>();
        }
    }
}
