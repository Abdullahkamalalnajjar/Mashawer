public class SendMessageCommand : IRequest<Response<MessageResponse>>
{
    public int ConversationId { get; set; }
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public string Content { get; set; }
}