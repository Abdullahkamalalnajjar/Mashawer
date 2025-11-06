using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly INotificationService _notificationService;
    private static readonly Dictionary<string, string> ConnectedUsers = new Dictionary<string, string>();
    public ChatHub(IChatService chatService, UserManager<ApplicationUser> userManager, INotificationService notificationService)
    {
        _chatService = chatService;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    // عند اتصال المستخدم
    public override async Task OnConnectedAsync()
    {

        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"================================={userId}=======================");

        if (!string.IsNullOrEmpty(userId))
        {
            lock (ConnectedUsers)
            {
                if (!ConnectedUsers.ContainsKey(userId))
                {
                    ConnectedUsers[userId] = Context.ConnectionId;
                }
            }
        }
        await base.OnConnectedAsync();
    }

    // عند انفصال المستخدم
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            lock (ConnectedUsers)
            {
                if (ConnectedUsers.ContainsKey(userId))
                {
                    ConnectedUsers.Remove(userId);
                }
            }
        }
        await base.OnDisconnectedAsync(exception);
    }

    // التحقق مما إذا كان المستخدم متصلًا
    public bool IsUserOnline(string userId)
    {
        return ConnectedUsers.ContainsKey(userId);
    }


    //Single

    public async Task SendMessage(int conversationId, string senderId, string receiverId, string content)
    {
        try
        {
            var messageDto = new MessageDto
            {
                ConversationId = conversationId,
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                IsRead = ConnectedUsers.ContainsKey(receiverId),
                SentAt = DateTime.UtcNow,

            };

            await _chatService.SaveMessageAsync(messageDto);

            var senderName = await _chatService.GetSenderNameAsync(senderId);
            //   var senderProfilePicture = await _chatService.GetSenderProfilePictureAsync(senderId);
            DateTime date = DateTime.UtcNow;

            // إرسال الرسالة عبر SignalR للطرفين
            await Clients.User(senderId).SendAsync("ReceiveMessages", conversationId, senderId, senderName, receiverId, content, date);
            await Clients.User(receiverId).SendAsync("ReceiveMessages", conversationId, senderId, senderName, receiverId, content, date);
            // التحقق مما إذا كان المستقبل غير متصل قبل إرسال الإشعار
            if (!ConnectedUsers.ContainsKey(receiverId))
            {
                var user = await _userManager.FindByIdAsync(receiverId);

                if (user != null && !string.IsNullOrEmpty(user.FCMToken))
                {
                    await _notificationService.SendChatNotificationAsync(
                        userId: user.Id,
                        fcmToken: user.FCMToken,
                        senderName: senderName,
                        messageContent: content,
                        additionalData: new Dictionary<string, string>
                        {
                            { "conversationId", $"{conversationId}" },
                            { "senderId", senderId },
                            { "status", "NewMessage" }
                        },
                        cancellationToken: default
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SendMessage: {ex.Message}");
            await Clients.User(senderId).SendAsync("ReceiveError", $"خطأ أثناء إرسال الرسالة: {ex.Message}");

            throw new HubException($"خطأ أثناء إرسال الرسالة: {ex.Message}");
        }
    }

    public async Task<IEnumerable<MessageDto>> GetMessages(int conversationId)
    {
        var messages = await _chatService.GetMessagesAsync(conversationId);
        await Clients.Caller.SendAsync("ReceiveMessages", messages);
        return messages;
    }


}
