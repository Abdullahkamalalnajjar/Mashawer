using Mashawer.Data.Enums;

namespace Mashawer.Core.Features.UserUpgradeRequests.Command.Models
{
    public class EditUserUpgradeRequestCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
        public RequestedRole? RequestedRole { get; set; }
        public string? Note { get; set; }
        public UpgradeRequestStatus? Status { get; set; }
        public string? Address { get; set; }

        // للمندوب فقط
        public string? TargetAgentId { get; set; }
    }
}
