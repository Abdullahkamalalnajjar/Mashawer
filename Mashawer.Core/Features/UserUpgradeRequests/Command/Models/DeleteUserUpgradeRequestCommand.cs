namespace Mashawer.Core.Features.UserUpgradeRequests.Command.Models
{
    public class DeleteUserUpgradeRequestCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
        public DeleteUserUpgradeRequestCommand(int id)
        {
            Id = id;
        }
    }
}
