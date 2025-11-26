namespace Mashawer.Core.Features.Orders.Commands.Models
{
    public class AddOrderTaskPhotosCommand : IRequest<Response<string>>
    {
        public int OrderTaskId { get; set; }
        public IFormFile? ItemPhotoBefore { get; set; }
        public IFormFile? ItemPhotoAfter { get; set; }
    }
}
