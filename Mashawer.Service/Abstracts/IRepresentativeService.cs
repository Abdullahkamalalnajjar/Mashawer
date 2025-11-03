namespace Mashawer.Service.Abstracts
{
    public interface IRepresentativeService
    {
        public Task<IEnumerable<RepresentativeDTO>> GetAllRepresentativesByAddressAsync(string address);
        public Task<IEnumerable<NearestRepresentativeDto>> GetNearestRepresentativeAsync(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude);
        public Task<string> UpdateLocation(string userId, double latitude, double longitude);
        public Task<string> UpdateInfo(string representativeId, IFormFile ImageUrl, string numberPanal, string model);
        public Task<string> MarkDriverArrivedAsync(int orderId);
        public Task<string> MarkIsClientLate(int orderId);

    }
}
