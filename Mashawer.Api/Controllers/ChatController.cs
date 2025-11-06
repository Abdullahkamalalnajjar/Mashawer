using Mashawer.Api.Base;
using Mashawer.Core.Features.Chat.Commands.Models;
using Mashawer.Core.Features.Chat.Queries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Mashawer.Apis.Controllers
{
    [Authorize]
    public class ChatController : AppBaseController
    {

        [HttpPost("create-conversation")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpGet("all-chats")]
        public async Task<IActionResult> GetAllConversationsAsync()
        {
            var request = new GetAllConversationsQuery();
            var response = await Mediator.Send(request);
            return NewResult(response);
        }
        [HttpGet("search-conversations")]
        public async Task<IActionResult> SearchConversationsAsync(string term)
        {
            var request = new SearchConversationQuery { SearchTerm = term };
            var response = await Mediator.Send(request);
            return NewResult(response);
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpGet("get-messages/{conversationId}")]
        public async Task<IActionResult> GetMessagesAsync([FromRoute] int conversationId)
        {
            var request = new GetMessagesQuery { ConversationId = conversationId };
            var response = await Mediator.Send(request);
            return NewResult(response);
        }
        [HttpPut("mark-all-messages-as-read/{conversationId}")]
        public async Task<IActionResult> MarkAllMessagesAsRead([FromRoute] int conversationId)
        {
            var command = new MarkAllMessageAsReadCommand { ConversationId = conversationId };
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
        [HttpGet("unread-chats-count")]
        public async Task<IActionResult> GetUnreadChatsCount()
        {
            var query = new GetUnreadChatsCountQuery();
            var response = await Mediator.Send(query);
            return NewResult(response);
        }

    }
}
