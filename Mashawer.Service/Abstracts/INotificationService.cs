namespace Mashawer.Service.Abstracts
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string userId, string fcmToken, string title, string body, CancellationToken cancellationToken);
        Task<IEnumerable<NotificationResponse>> GetNotificationForUserAsync(string userId);
        Task<string> DeleteAllNotificationAsync(string userId);
        Task<string> DeleteNotificationAsync(int notificationId, string userId);
        Task<string> MarkAsReadAsync(int notificationId);
        Task SendChatNotificationAsync(
            string userId,
            string fcmToken,
            string senderName,
            string messageContent,
            Dictionary<string, string> additionalData,
            CancellationToken cancellationToken = default);
    }
}
