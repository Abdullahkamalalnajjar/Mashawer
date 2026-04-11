namespace Mashawer.Service.Abstracts
{
    public interface IRepresentativeService
    {
        public Task<IEnumerable<RepresentativeDTO>> GetAllRepresentativesByAddressAsync(string address);
        public Task<IEnumerable<NearestRepresentativeDto>> GetNearestRepresentatives(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude);
        public Task<string> UpdateLocation(string userId, double Tolatitude, double fromLatitude, double Tolongitude, double fromLongitude);
        public Task<string> UpdateInfo(string representativeId, IFormFile vehicalPicture, string vehicalNumber, string type, string vehicleColor);

        public Task<string> MarkDriverArrivedAsync(int orderId);
        public Task<string> MarkIsClientLate(int orderId);
        public Task<RepresentativeInfoDto> GetRepresentativeInfoAsync(string representativeId);
        public Task<IEnumerable<RepresentativeDTO>> GetAllActiveRepresentativesByAddressAsync(string address, bool isActive);


    }
}
