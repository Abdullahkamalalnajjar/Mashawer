namespace Mashawer.Core.Features.Chat.Commands.Results
{
    public class CreateConversationResponse
    {
        public int ConversationId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        //    public string UserImage { get; set; }

    }

}