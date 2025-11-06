using MediatR;
using Mashawer.Data.Dtos.Chat;
using Mashawer.Core.Bases;

public class GetMessagesQuery : IRequest<Response<IEnumerable<MessageDto>>>
{
    public int ConversationId { get; set; }
}