
namespace Mashawer.Service.Implementations
{
    public class UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IRoleService roleService, ApplicationDbContext context) : IUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IRoleService _roleService = roleService;
        private readonly ApplicationDbContext _context = context;

        public async Task<string> CreateUserAsync(string email, string firstName, string LastName, string password, IList<string> roles, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                Email = email,
                FirstName = firstName,
                LastName = LastName,
                UserName = email,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, roles);

                return "Created";
            }
            if (result.Errors.Any())
            {
                return result.Errors.FirstOrDefault()?.Description ?? "ErrorCreatingUser";
            }
            return "UnknownError";
        }

        public async Task<string> UpdateUserAsync(ApplicationUser user, IList<string> roles)
        {
            await _userManager.UpdateAsync(user);
            await _userManager.AddToRolesAsync(user, roles);
            return "Updated";

        }
        public async Task<IEnumerable<UserResponse>> GetAllUsers(string? address)
        {
            var query =
                from u in _context.Users
                join w in _context.Wallets on u.Id equals w.UserId into wallets
                from w in wallets.DefaultIfEmpty()
                join ur in _context.UserRoles on u.Id equals ur.UserId into userRoles
                from ur in userRoles.DefaultIfEmpty()
                join r in _context.Roles on ur.RoleId equals r.Id into roles
                from r in roles.DefaultIfEmpty()
                select new { u, w, r };

            // ✅ فلترة على Address فقط
            if (!string.IsNullOrWhiteSpace(address))
            {
                query = query.Where(x =>
                    x.u.Address != null &&
                    x.u.Address.Contains(address)
                );
            }

            var users = await query
                .GroupBy(x => new
                {
                    x.u.Id,
                    x.u.PhoneNumber,
                    x.u.ProfilePictureUrl,
                    x.u.Email,
                    x.u.FirstName,
                    x.u.LastName,
                    x.u.UserType,
                    x.u.Address,
                    WalletId = (int?)x.w.Id,
                    WalletBalance = (decimal?)x.w.Balance
                })
                .Select(g => new UserResponse(
                    g.Key.Id,
                    g.Key.PhoneNumber,
                    g.Key.ProfilePictureUrl,
                    g.Key.Email,
                    g.Key.FirstName,
                    g.Key.LastName,
                    g.Key.UserType.ToString(),
                    g.Key.Address,        // ✅ العنوان الأساسي
                    null,                 // AgentAddress
                    null,                 // RepresentativeAddress
                    null,
                    null,
                    null,
                    null,
                    g.Key.WalletId,
                    g.Key.WalletBalance,
                    g.Where(x => x.r != null)
                     .Select(x => x.r!.Name)
                     .Distinct()
                ))
                .ToListAsync();

            return users;
        }




        public async Task<UserResponse?> GetUserById(string id)
        {
            var user = await (
                from u in _context.Users
                join w in _context.Wallets on u.Id equals w.UserId into wallets
                from w in wallets.DefaultIfEmpty()
                join ur in _context.UserRoles on u.Id equals ur.UserId into userRoles
                from ur in userRoles.DefaultIfEmpty()
                join r in _context.Roles on ur.RoleId equals r.Id into roles
                from r in roles.DefaultIfEmpty()
                where u.Id == id
                group r by new
                {
                    u.Id,
                    u.PhoneNumber,
                    u.ProfilePictureUrl,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.UserType,
                    u.Address,                     // ✅ أضفناها
                    u.AgentAddress,
                    u.RepresentativeAddress,
                    u.RepresentativeFromLongitude,
                    u.RepresentativeToLongitude,
                    u.RepresentativeFromLatitude,
                    u.RepresentativeToLatitude,
                    WalletId = (int?)w.Id,
                    WalletBalance = (decimal?)w.Balance
                } into g
                select new UserResponse(
                    g.Key.Id,
                    g.Key.PhoneNumber,
                    g.Key.ProfilePictureUrl,
                    g.Key.Email,
                    g.Key.FirstName,
                    g.Key.LastName,
                    g.Key.UserType.ToString(),
                    g.Key.Address,                 // ✅ شغالة
                    g.Key.AgentAddress,
                    g.Key.RepresentativeAddress,
                    g.Key.RepresentativeFromLongitude,
                    g.Key.RepresentativeToLongitude,
                    g.Key.RepresentativeFromLatitude,
                    g.Key.RepresentativeToLatitude,
                    g.Key.WalletId,
                    g.Key.WalletBalance,
                    g.Where(r => r != null)
                     .Select(r => r!.Name)
                     .Distinct()
                     .ToList()
                )
            ).SingleOrDefaultAsync();

            return user;
        }

        public async Task<ApplicationUser> GetUserProfileAsync(string userId)
            => await _unitOfWork.Users.GetTableNoTracking()
            .Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
        public async Task<string> UpdateProfileUser(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
            return "Updated";
        }

        public async Task<string> DeleteUserWithReasone(ApplicationUser applicationUser, string reason, CancellationToken cancellationToken)
        {
            applicationUser.IsDisable = true;
            await _userManager.UpdateAsync(applicationUser);
            var deletedRecord = new DeletedRecord
            {
                Email = applicationUser.Email,
                Reason = reason,
                DeletedAt = DateTime.UtcNow
            };
            await _unitOfWork.DeleteRecoreds.AddAsync(deletedRecord, cancellationToken);
            return "Deleted";

        }

        public async Task<IEnumerable<UserResponse>> GetAllAgnetAsync()
        {
            var users = await (
                 from u in _context.Users
                 where u.UserType == UserType.Agent // ✅ تصفية المستخدمين حسب النوع
                 join w in _context.Wallets on u.Id equals w.UserId into wallets
                 from w in wallets.DefaultIfEmpty()
                 join ur in _context.UserRoles on u.Id equals ur.UserId into userRoles
                 from ur in userRoles.DefaultIfEmpty()
                 join r in _context.Roles on ur.RoleId equals r.Id into roles
                 from r in roles.DefaultIfEmpty()
                 group r by new
                 {
                     u.Id,
                     u.PhoneNumber,
                     u.ProfilePictureUrl,
                     u.Email,
                     u.FirstName,
                     u.LastName,
                     u.UserType,
                     u.Address,
                     u.AgentAddress,// ✅ إضافة العنوان الخاص بالوكيل,

                     u.RepresentativeAddress,
                     u.RepresentativeFromLongitude,
                     u.RepresentativeToLongitude,
                     u.RepresentativeFromLatitude,
                     u.RepresentativeToLatitude,
                     WalletId = (int?)w.Id,
                     WalletBalance = (decimal?)w.Balance
                 } into g
                 select new UserResponse(
                     g.Key.Id,
                     g.Key.PhoneNumber,
                     g.Key.ProfilePictureUrl,
                     g.Key.Email,
                     g.Key.FirstName,
                     g.Key.LastName,
                     g.Key.UserType.ToString(), // ✅ تحويل enum إلى string
                        g.Key.Address,
                     g.Key.AgentAddress,
                     g.Key.RepresentativeAddress, // إضافة العنوان الخاص بالمندوب
                     g.Key.RepresentativeFromLongitude,
                     g.Key.RepresentativeToLongitude,
                     g.Key.RepresentativeFromLatitude,
                     g.Key.RepresentativeToLatitude,
                     g.Key.WalletId,
                     g.Key.WalletBalance,
                     g.Where(role => role != null).Select(role => role.Name).Distinct()
                 )
             ).ToListAsync();

            return users;

        }

        public async Task<IEnumerable<UserResponse>> GetAllRepresentativeAsync(string? address)
        {
            var query =
                from u in _context.Users
                where u.UserType == UserType.Representative
                join w in _context.Wallets on u.Id equals w.UserId into wallets
                from w in wallets.DefaultIfEmpty()
                join ur in _context.UserRoles on u.Id equals ur.UserId into userRoles
                from ur in userRoles.DefaultIfEmpty()
                join r in _context.Roles on ur.RoleId equals r.Id into roles
                from r in roles.DefaultIfEmpty()
                select new { u, w, r };

            // ✅ إضافة الفلترة حسب العنوان لو مش فاضي
            if (!string.IsNullOrEmpty(address))
            {
                query = query.Where(x =>
                    (x.u.Address != null && x.u.Address.Contains(address)));
            }

            var users = await (
                from x in query
                group x.r by new
                {
                    x.u.Id,
                    x.u.PhoneNumber,
                    x.u.ProfilePictureUrl,
                    x.u.Email,
                    x.u.FirstName,
                    x.u.LastName,
                    x.u.UserType,
                    x.u.Address,
                    x.u.AgentAddress,
                    x.u.RepresentativeAddress,
                    x.u.RepresentativeFromLongitude,
                    x.u.RepresentativeToLongitude,
                    x.u.RepresentativeFromLatitude,
                    x.u.RepresentativeToLatitude,
                    WalletId = (int?)x.w.Id,
                    WalletBalance = (decimal?)x.w.Balance
                } into g
                select new UserResponse(
                    g.Key.Id,
                    g.Key.PhoneNumber,
                    g.Key.ProfilePictureUrl,
                    g.Key.Email,
                    g.Key.FirstName,
                    g.Key.LastName,
                    g.Key.UserType.ToString(),
                    g.Key.Address,
                    g.Key.AgentAddress,
                    g.Key.RepresentativeAddress,
                    g.Key.RepresentativeFromLongitude,
                    g.Key.RepresentativeToLongitude,
                    g.Key.RepresentativeFromLatitude,
                    g.Key.RepresentativeToLatitude,
                    g.Key.WalletId,
                    g.Key.WalletBalance,
                    g.Where(role => role != null).Select(role => role.Name).Distinct()
                )
            ).ToListAsync();

            return users;
        }

        public async Task<string> DeleteUserAsync(ApplicationUser applicationUser)
        {
            applicationUser.IsDisable = true;
            await _userManager.UpdateAsync(applicationUser);
            return "Deleted";
        }
    }

}
