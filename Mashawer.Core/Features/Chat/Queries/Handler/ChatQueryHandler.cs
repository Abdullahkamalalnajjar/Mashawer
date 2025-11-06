using Mashawer.Core.Features.Chat.Queries.Models;
using Mashawer.Data.Dtos.Chat;

namespace Mashawer.Core.Features.Chat.Queries.Handler
{
    public class ChatQueryHandler(IChatService chatService, ICurrentUserService currentUserService) : ResponseHandler,
        IRequestHandler<GetAllConversationsQuery, Response<IEnumerable<ConversationDTO>>>,
        IRequestHandler<SearchConversationQuery, Response<IEnumerable<ConversationDTO>>>,
        IRequestHandler<GetMessagesQuery, Response<IEnumerable<MessageDto>>>,
        IRequestHandler<GetUnreadChatsCountQuery, Response<int>>
    {
        private readonly IChatService _chatService = chatService;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<Response<IEnumerable<ConversationDTO>>> Handle(GetAllConversationsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var conversations = await _chatService.GetAllConversationsAsync(userId!);
            return (Success(conversations, "Conversations retrieved successfully."));
        }

        public async Task<Response<IEnumerable<ConversationDTO>>> Handle(SearchConversationQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var conversations = await _chatService.GetAllConversationsAsync(userId!, request.SearchTerm);
            return Success(conversations, "Conversations retrieved successfully.");
        }

        public async Task<Response<IEnumerable<MessageDto>>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var messages = await _chatService.GetMessagesAsync(request.ConversationId);
            if (messages == null || !messages.Any())
            {
                return NotFound<IEnumerable<MessageDto>>("No messages found for this conversation.");
            }
            return Success(messages, "Messages retrieved successfully.");
        }

        public async Task<Response<int>> Handle(GetUnreadChatsCountQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var result = await _chatService.GetUnreadChatsCountAsync(userId);
            return Success(result);
        }
    }
}
