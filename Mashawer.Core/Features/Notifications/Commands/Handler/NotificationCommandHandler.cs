using Mashawer.Core.Features.Notifications.Commands.Models;

namespace Mashawer.Core.Features.Notifications.Commands.Handler
{
    public class NotificationCommandHandler(INotificationService notificationService, ICurrentUserService currentUserService) : ResponseHandler,
        IRequestHandler<DeleteAllNotificationCommand, Response<string>>,
        IRequestHandler<DeleteNotificationCommand, Response<string>>,
        IRequestHandler<MarkAsReadCommand, Response<string>>
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<Response<string>> Handle(DeleteAllNotificationCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var result = await _notificationService.DeleteAllNotificationAsync(userId);
            return result switch
            {
                "NotFound" => BadRequest<string>("No notifications found for the user."),
                "Deleted" => Success<string>("All notifications deleted successfully."),
                _ => UnprocessableEntity<string>("An error occurred while deleting notifications.")
            };
        }

        public async Task<Response<string>> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var result = await _notificationService.DeleteNotificationAsync(request.NotificationId, userId);
            return result switch
            {
                "NotFound" => BadRequest<string>("Notification not found."),
                "Deleted" => Success<string>("Notification deleted successfully."),
                _ => UnprocessableEntity<string>("An error occurred while deleting the notification.")
            };
        }

        public async Task<Response<string>> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var result = await _notificationService.MarkAsReadAsync(request.NotificationId);
            return result switch
            {
                "NotFound" => BadRequest<string>("Notification not found."),
                "MarkedAsRead" => Success<string>("Notification marked as read successfully."),
                _ => UnprocessableEntity<string>("An error occurred while marking the notification as read.")
            };
        }
    }
}
