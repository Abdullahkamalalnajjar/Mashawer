using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.Admin.Command.Models
{
    public class AcceptOrRejectRequestCommand : IRequest<Response<string>>
    {
        public int RequestId { get; set; }
        public UpgradeRequestStatus Status { get; set; }

    }
}
