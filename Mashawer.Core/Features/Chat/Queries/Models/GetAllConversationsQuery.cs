using Mashawer.Core.Bases;
using Mashawer.Data.Dtos.Chat;
using MediatR;

public class GetAllConversationsQuery : IRequest<Response<IEnumerable<ConversationDTO>>>
{
  
}