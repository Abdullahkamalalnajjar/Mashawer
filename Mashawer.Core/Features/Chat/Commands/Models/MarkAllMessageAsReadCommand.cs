using Mashawer.Core.Bases;
using MediatR;

namespace Mashawer.Core.Features.Chat.Commands.Models
{
    public class MarkAllMessageAsReadCommand : IRequest<Response<string>>
    {
        public int ConversationId { get; set; }
    }
}
