
namespace Mashawer.Service.Implementations
{
    public class RoleService(RoleManager<ApplicationRole> roleManager, IMapper mapper, ApplicationDbContext context) : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IMapper _mapper = mapper;
        private readonly ApplicationDbContext _context = context;

        public async Task<string> AddRoleWithPermissionAsync(string name, IList<string> permissions)
        {
            var roleIsExist = await _roleManager.RoleExistsAsync(name);
            if (roleIsExist)
                return "IsExist";
            var allowPermissions = Permissions.GetAllPermissions();
            if (permissions.Except(allowPermissions).Any())// معناها لو في اي حاجه بعتها مش موجوده داخل allowPermission هيطلع true         
                return "InvalidPermissions";
            var role = new ApplicationRole
            {
                Name = name,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                var addPermissions = permissions.Select(x => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
                });
                await _context.AddRangeAsync(addPermissions);
                await _context.SaveChangesAsync();
            }
            return "Created";
        }
        public async Task<IEnumerable<RoleResponse>> GetAllRolesAsync(bool? includeDeleted = false)
        {
            var roles = await _roleManager.Roles.Where(x => !x.IsDefualt && (!x.IsDeleted || (includeDeleted == true)))
                .ToListAsync();
            var response = _mapper.Map<IEnumerable<RoleResponse>>(roles);
            return response;
        }
        public async Task<RoleDetailsResponse> GetRolesDetailsAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return null!;
            var permissions = await _roleManager.GetClaimsAsync(role);
            var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted, permissions.Select(x => x.Value));
            return response;
        }

        public async Task<RoleDetailsResponse> TestGetRoleDetailsAsync(string roleId)
        {
            if (await _roleManager.FindByIdAsync(roleId) is not { } role)
                return null!;
            var permissions = await _roleManager.GetClaimsAsync(role);
            var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted, permissions.Select(x => x.Value));
            return response;

        }
        public async Task<string> UpdateRoleWithPermissionAsync(string id, string name, IList<string> permissions)
        {
            if (await _roleManager.FindByIdAsync(id) is not { } role)
                return "NotFound";
            var roleIsExist = await _roleManager.Roles.AnyAsync(x => x.Name == name && x.Id != id);
            if (roleIsExist)
                return "IsExist";
            role.Name = name;

            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                var allPermissions = Permissions.GetAllPermissions();
                if (permissions.Except(allPermissions).Any())
                    return "InvalidPermissions";
                var currentPermissions = await _context.RoleClaims
                    .Where(x => x.RoleId == id && x.ClaimType == Permissions.Type).Select(x => x.ClaimValue)
                    .ToListAsync();
                var newPermissions = permissions.Except(currentPermissions).Select(x => new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimValue = x,
                    ClaimType = Permissions.Type
                });
                var removedPermissions = currentPermissions.Except(permissions);
                await _context.RoleClaims
                    .Where(x => x.RoleId == id && removedPermissions.Contains(x.ClaimValue))
                    .ExecuteDeleteAsync();
                await _context.AddRangeAsync(newPermissions);
                await _context.SaveChangesAsync();
                return "Updated";
            }
            return "Error";
        }
        public async Task<string> TestUpdateRoleWithPermissions(string id, string name, IList<string> permissions)
        {
            if (await _roleManager.FindByIdAsync(id) is not { } role)
                return "NotFound";
            var roleIsExisted = await _roleManager.Roles.AnyAsync(x => x.Name.Equals(name) && x.Id != id);
            if (roleIsExisted)
                return "IsExisted";
            role.Name = name;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                var currentPermissions = await _context.RoleClaims.Where(x => x.RoleId == id && x.ClaimType == Permissions.Type)
                    .Select(x => x.ClaimValue).ToListAsync();
                var newPermissions = permissions.Except(currentPermissions);
                var removedPermissions = currentPermissions.Except(newPermissions);

                await _context.RoleClaims.Where(x => x.RoleId == id && removedPermissions.Contains(x.ClaimValue)).ExecuteDeleteAsync();
                var addPermissions = newPermissions.Select(x => new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = Permissions.Type,
                    ClaimValue = x
                });
                await _context.AddRangeAsync(addPermissions);
                await _context.SaveChangesAsync();
                return "Updated";
            }
            return "Error";
        }
    }
}
