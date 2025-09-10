using FirebaseAdmin.Messaging;

namespace Mashawer.Service.Implementations
{
    public class NotificationService(IUnitOfWork unitOfWork) : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<string> DeleteAllNotificationAsync(string userId)
        {
            var notifications = _unitOfWork.Notifications.GetTableNoTracking()
                .Where(n => n.UserId == userId);
            if (!notifications.Any())
            {
                return "NotFound";
            }
            await notifications.ExecuteDeleteAsync();
            return "Deleted";

        }

        public async Task<string> DeleteNotificationAsync(int notificationId, string userId)
        {
            var notification = await _unitOfWork.Notifications.GetTableNoTracking()
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
            if (notification == null)
                return "NotFound";
            await _unitOfWork.Notifications.Delete(notification);
            await _unitOfWork.CompeleteAsync();
            return "Deleted";
        }

        public async Task<IEnumerable<NotificationResponse>> GetNotificationForUserAsync(string userId)
        {
            return await _unitOfWork.Notifications.GetTableNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.Timestamp)
                .Select(n => new NotificationResponse
                {
                    Id = n.Id,
                    Title = n.Title,
                    Body = n.Body,
                    Timestamp = n.Timestamp,
                    IsRead = n.IsRead,
                })
                .ToListAsync();
        }
        public async Task<string> MarkAsReadAsync(int notificationId)
        {
            var notification = await _unitOfWork.Notifications.GetTableAsTracking()
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification == null)
                return "NotFound";

            notification.IsRead = true;
            await _unitOfWork.CompeleteAsync();

            return "MarkedAsRead";
        }

        public async Task SendNotificationAsync(string userId, string fcmToken, string title, string body, CancellationToken cancellationToken)
        {
            TimeZoneInfo egyptZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            DateTime egyptTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptZone);

            var message = new Message
            {
                Token = fcmToken,
                Data = new Dictionary<string, string>
                {
                    { "status", "Notification" }
                },
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
                }
            };
            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                var notification = new UserNotification
                {
                    Title = title,
                    Body = body,
                    UserId = userId,
                    Timestamp = egyptTime,
                };
                await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
                await _unitOfWork.CompeleteAsync();
                Console.WriteLine($"✅ Notification sent: {response}");
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"❌ Error sending notification: {ex.Message}");
                Console.WriteLine($"Token: {fcmToken}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }

        public async Task SendChatNotificationAsync(
            string userId,
            string fcmToken,
            string senderName,
            string messageContent,
            Dictionary<string, string> additionalData,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Save notification in database
                var notification = new UserNotification
                {
                    UserId = userId,
                    Title = $"رسالة جديدة من {senderName}",
                    Body = messageContent,
                    Timestamp = DateTime.UtcNow,
                    IsRead = false
                };

                await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
                await _unitOfWork.CompeleteAsync();

                if (!string.IsNullOrEmpty(fcmToken))
                {
                    var message = new Message
                    {
                        Notification = new FirebaseAdmin.Messaging.Notification
                        {
                            Title = $"رسالة جديدة من {senderName}",
                            Body = messageContent
                        },
                        Token = fcmToken,
                        Data = additionalData
                    };

                    await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error sending chat notification: {ex.Message}");
                throw;
            }
        }
    }
}
