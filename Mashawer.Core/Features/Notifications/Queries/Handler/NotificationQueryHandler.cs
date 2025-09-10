using Mashawer.Core.Features.Notifications.Queries.Models;

namespace Mashawer.Core.Features.Notifications.Queries.Handler
{
    public class NotificationQueryHandler(INotificationService notificationService, ICurrentUserService currentUserService) : ResponseHandler,
        IRequestHandler<GetNotificationOfUserQuery, Response<IEnumerable<NotificationResponse>>>
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<Response<IEnumerable<NotificationResponse>>> Handle(GetNotificationOfUserQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var notifications = await _notificationService.GetNotificationForUserAsync(userId);
            return Success<IEnumerable<NotificationResponse>>(notifications);

        }
    }
}
