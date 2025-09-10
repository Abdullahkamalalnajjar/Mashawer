namespace Mashawer.Core.Features.Notifications.Commands.Models
{
    public class DeleteNotificationCommand : IRequest<Response<string>>
    {
        public int NotificationId { get; set; }
        public DeleteNotificationCommand(int notificationId)
        {
            NotificationId = notificationId;
        }
    }
}
