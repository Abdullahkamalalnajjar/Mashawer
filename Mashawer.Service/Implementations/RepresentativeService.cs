namespace Mashawer.Service.Implementations
{
    public class RepresentativeService(IUnitOfWork unitOfWork) : IRepresentativeService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IEnumerable<RepresentativeDTO>> GetAllRepresentativesByAddressAsync(string address) // بتجيب المناديب حسب موقع ال شغال فيه وليس اقامته
        {
            return await _unitOfWork.Users.GetTableNoTracking()
                .Where(x => x.UserType == UserType.Representative && x.RepresentativeAddress == address)
                .Select(x => new RepresentativeDTO
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    ProfilePictureUrl = x.ProfilePictureUrl,
                    Address = x.RepresentativeAddress
                }).ToListAsync();
        }
    }
}
