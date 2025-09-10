namespace Mashawer.Core.Features.Notifications.Commands.Models
{
    public class MarkAsReadCommand : IRequest<Response<string>>
    {
        public int NotificationId { get; set; }
        public MarkAsReadCommand(int notificationId)
        {
            NotificationId = notificationId;
        }
    }
}
