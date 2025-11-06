using Mashawer.Core.Features.Chat.Commands.Results;

public class CreateConversationCommand : IRequest<Response<CreateConversationResponse>>
{
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
}