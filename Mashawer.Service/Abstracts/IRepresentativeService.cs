namespace Mashawer.Service.Abstracts
{
    public interface IRepresentativeService
    {
        public Task<IEnumerable<RepresentativeDTO>> GetAllRepresentativesByAddressAsync(string address);
        public Task<IEnumerable<NearestRepresentativeDto>> GetNearestRepresentativeAsync(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude);

    }
}
