namespace Mashawer.Core.Features.UserDailyDiscounts.Command.Models
{
    public class MarkUsedCommand : IRequest<Response<string>>
    {
        public MarkUsedCommand(List<int> ids)
        {
            Ids = ids;
        }

        public List<int> Ids { get; set; }
    }

}
