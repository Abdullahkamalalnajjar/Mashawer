

namespace Mashawer.Service.Implementations
{
    public class ClaimService(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager) : IClaimService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

        public ClaimResponse GetClaim()
        {
            var permissions = Permissions.GetAllPermissions();
            return new ClaimResponse(Permissions.Type, permissions!);
        }
        public async Task<ClaimResponse> GetClaimByRole(string roleId)
        {
            if (await _roleManager.FindByIdAsync(roleId) is not { } role)
                return null!;
            var permissions = await _context.RoleClaims.Where(x => x.RoleId == roleId).Select(x => x.ClaimValue).ToListAsync();
            return new ClaimResponse(Permissions.Type, permissions!);

        }
    }
}
