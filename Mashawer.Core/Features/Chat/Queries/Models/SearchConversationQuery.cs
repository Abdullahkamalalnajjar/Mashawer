using Mashawer.Core.Bases;
using Mashawer.Data.Dtos.Chat;
using MediatR;

namespace Mashawer.Core.Features.Chat.Queries.Models
{
    public class SearchConversationQuery : IRequest<Response<IEnumerable<ConversationDTO>>>
    {
        public string SearchTerm { get; set; }
    }
}
