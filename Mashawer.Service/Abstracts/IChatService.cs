
namespace Mashawer.Service.Abstracts
{
    public interface IChatService
    {
        Task<Conversation> CreateOrGetConversationAsync(string senderId, string receiverId);
        Task<ChatMessage> SendMessageAsync(int conversationId, string senderId, string receiverId, string content);
        Task<IEnumerable<ConversationDTO>> GetAllConversationsAsync(string userId, string term = null);
        Task<IEnumerable<MessageDto>> GetMessagesAsync(int conversationId);
        Task SaveMessageAsync(MessageDto messageDto);
        Task<IEnumerable<ConversationDTO>> GetAllConversationsAsync(string userId);
        Task<string> GetSenderNameAsync(string senderId);
        //  Task<string> GetSenderProfilePictureAsync(string senderId);
        Task<string> MarkAllMessagesAsReadAsync(int conversationId, string userId);
        Task<int> GetUnreadChatsCountAsync(string userId);
    }
}
