using Mashawer.Core.Features.Chat.Commands.Models;
using Mashawer.Core.Features.Chat.Commands.Results;
using Microsoft.AspNetCore.SignalR;

public class ChatCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IChatService chatService, IHubContext<ChatHub> hubContext)
    : ResponseHandler, IRequestHandler<CreateConversationCommand, Response<CreateConversationResponse>>,
    IRequestHandler<SendMessageCommand, Response<MessageResponse>>,
   IRequestHandler<MarkAllMessageAsReadCommand, Response<string>>

{
    private readonly IChatService _chatService = chatService;
    private readonly IHubContext<ChatHub> _hubContext = hubContext;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<Response<CreateConversationResponse>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _chatService.CreateOrGetConversationAsync(request.SenderId, request.ReceiverId);

        // الحصول على بيانات المرسل والمستقبل
        var sender = await _unitOfWork.Users.FindAsync(d => d.Id == request.SenderId);
        var recipient = await _unitOfWork.Users.FindAsync(d => d.Id == request.ReceiverId);
        var response = new CreateConversationResponse
        {
            ConversationId = conversation.Id,
            SenderId = sender?.Id ?? string.Empty,
            SenderName = sender?.FullName ?? "Unknown",
            ReceiverId = recipient?.Id ?? string.Empty,
            ReceiverName = recipient?.FullName ?? "Unknown"
        };
        return Success(response, "Conversation created successfully.");

    }

    public async Task<Response<MessageResponse>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var sender = await _unitOfWork.Users.FindAsync(d => d.Id == request.SenderId);
        var recipient = await _unitOfWork.Users.FindAsync(d => d.Id == request.ReceiverId);
        if (sender == null || recipient == null)
        {
            return BadRequest<MessageResponse>("Sender or receiver not found");
        }
        var message = await _chatService.SendMessageAsync(request.ConversationId, request.SenderId, request.ReceiverId, request.Content);
        if (message == null)
        {
            return BadRequest<MessageResponse>("Failed to send message");
        }
        // إرسال الرسالة عبر SignalR
        await _hubContext.Clients.User(request.ReceiverId)
            .SendAsync("ReceiveMessage", new
            {
                conversationId = message.ConversationId,
                messageDetails = new
                {
                    sender = new
                    {
                        senderId = sender.Id,
                        senderName = sender.UserName,
                        senderImage = sender.ProfilePictureUrl ?? "default_image_url"
                    },
                    recipient = new
                    {
                        recipientId = recipient.Id,
                        recipientName = recipient.UserName,
                        recipientImage = recipient.ProfilePictureUrl ?? "default_image_url"
                    },
                    content = message.Content,
                    sentAt = message.SentAt
                }
            });
        var messageResponse = new MessageResponse
        {
            ConversationId = message.ConversationId,
            Sender = new SenderResponse
            {
                SenderId = sender.Id,
                SenderName = sender.UserName,
                SenderImage = sender.ProfilePictureUrl ?? "default_image_url"
            },
            Recipient = new RecipientResponse
            {
                RecipientId = recipient.Id,
                RecipientName = recipient.UserName,
                RecipientImage = recipient.ProfilePictureUrl ?? "default_image_url"
            },
            Content = message.Content,
            SentAt = message.SentAt
        };
        return Success(messageResponse, "Message sent successfully.");

    }
    public async Task<Response<string>> Handle(MarkAllMessageAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var result = await _chatService.MarkAllMessagesAsReadAsync(request.ConversationId, userId!);
        if (result == "Marked")
            return Success(result, "All messages marked as read successfully.");
        return BadRequest<string>("Failed to mark messages as read. Please try again later.");
    }
}