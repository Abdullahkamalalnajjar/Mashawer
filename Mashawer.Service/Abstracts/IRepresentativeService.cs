namespace Mashawer.Service.Abstracts
{
    public interface IRepresentativeService
    {
        public Task<IEnumerable<RepresentativeDTO>> GetAllRepresentativesByAddressAsync(string address);
    }
}
