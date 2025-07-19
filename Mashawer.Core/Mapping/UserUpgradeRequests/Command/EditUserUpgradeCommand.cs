using Mashawer.Core.Features.UserUpgradeRequests.Command.Models;

namespace Mashawer.Core.Mapping.UserUpgradeRequests
{
    public partial class UserUpgradeRequestProfile
    {
        public void EditUserUpgradeCommand_Mapper()
        {
            CreateMap<EditUserUpgradeRequestCommand, UserUpgradeRequest>();
        }
    }
}
