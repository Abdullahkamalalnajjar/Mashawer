namespace Mashawer.Core.Features.Representatives.Command.Models
{
    public class UpdateRepresentativeInfoCommand : IRequest<Response<string>>
    {
        public string RepresentativeId { get; set; }
        public IFormFile? VehicleUrl { get; set; }
        public string? VehicleNumber { get; set; }
        public string? VehicleType { get; set; }
    }


}
