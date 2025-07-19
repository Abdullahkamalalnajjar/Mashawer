    
namespace Mashawer.Service.Abstracts
{
    public interface IClaimService
    {
        public ClaimResponse GetClaim();
        public Task<ClaimResponse> GetClaimByRole(string roleId);
    }
}
