
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
        public async Task<IEnumerable<UserResponse>> GetAllUsers()
        {
            var users = await (
                from u in _context.Users
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
                    u.AgentAddress,
                    u.RepresentativeAddress,
                    u.RepresentativeLongitude,
                    u.RepresentativeLatitude

                } into g
                select new UserResponse(
                    g.Key.Id,
                    g.Key.PhoneNumber,
                    g.Key.ProfilePictureUrl,
                    g.Key.Email,
                    g.Key.FirstName,
                    g.Key.LastName,
                    g.Key.UserType.ToString(),
                    g.Key.AgentAddress,
                    g.Key.RepresentativeAddress,
                    g.Key.RepresentativeLatitude,
                    g.Key.RepresentativeLongitude,
                    g.Where(role => role != null).Select(role => role.Name).Distinct()
                )
            ).ToListAsync();

            return users;
        }



        public async Task<UserResponse?> GetUserById(string id)
        {
            var user = await (
                from u in _context.Users
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
                    u.AgentAddress,
                    u.RepresentativeAddress,
                    u.RepresentativeLongitude,
                    u.RepresentativeLatitude
                } into g
                select new UserResponse(
                    g.Key.Id,
                    g.Key.PhoneNumber,
                    g.Key.ProfilePictureUrl,
                    g.Key.Email,
                    g.Key.FirstName,
                    g.Key.LastName,
                    g.Key.UserType.ToString(),
                    g.Key.AgentAddress,
                    g.Key.RepresentativeAddress,
                    g.Key.RepresentativeLatitude,
                    g.Key.RepresentativeLongitude,
                    g.Where(r => r != null).Select(r => r.Name).Distinct().ToList()
                )
            ).SingleOrDefaultAsync();

            return user;
        }

        public async Task<ApplicationUser> GetUserProfileAsync(string userId)
            => await _unitOfWork.Users.GetTableNoTracking()
            .Where(x => x.Id.Equals(userId)).SingleAsync();
        public async Task<string> UpdateProfileUser(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
            return "Updated";
        }

        public async Task<string> DeleteUserWithReasone(ApplicationUser applicationUser, string reason, CancellationToken cancellationToken)
        {

            await _unitOfWork.Users.Delete(applicationUser);
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
                     u.AgentAddress,// ✅ إضافة العنوان الخاص بالوكيل,
                     u.RepresentativeAddress,
                     u.RepresentativeLongitude,
                     u.RepresentativeLatitude
                 } into g
                 select new UserResponse(
                     g.Key.Id,
                     g.Key.PhoneNumber,
                     g.Key.ProfilePictureUrl,
                     g.Key.Email,
                     g.Key.FirstName,
                     g.Key.LastName,
                     g.Key.UserType.ToString(), // ✅ تحويل enum إلى string
                     g.Key.AgentAddress,
                     g.Key.RepresentativeAddress, // إضافة العنوان الخاص بالمندوب
                     g.Key.RepresentativeLongitude,
                     g.Key.RepresentativeLatitude,
                     g.Where(role => role != null).Select(role => role.Name).Distinct()
                 )
             ).ToListAsync();

            return users;

        }

        public async Task<IEnumerable<UserResponse>> GetAllRepresentativeAsync()
        {
            var users = await (
         from u in _context.Users
         where u.UserType == UserType.Representative // ✅ تصفية المستخدمين حسب النوع
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
             u.AgentAddress,
             u.RepresentativeAddress,
             u.RepresentativeLongitude,
             u.RepresentativeLatitude
         } into g
         select new UserResponse(
             g.Key.Id,
             g.Key.PhoneNumber,
             g.Key.ProfilePictureUrl,
             g.Key.Email,
             g.Key.FirstName,
             g.Key.LastName,
             g.Key.UserType.ToString(), // ✅ تحويل enum إلى string
             g.Key.AgentAddress, // إضافة العنوان الخاص بالوكيل
             g.Key.RepresentativeAddress, // إضافة العنوان الخاص بالمندوب
             g.Key.RepresentativeLongitude,
             g.Key.RepresentativeLatitude,
             g.Where(role => role != null).Select(role => role.Name).Distinct()
         )
     ).ToListAsync();

            return users;
        }

        public async Task<string> DeleteUserAsync(ApplicationUser applicationUser)
        {
            await _userManager.DeleteAsync(applicationUser);
            return "Deleted";
        }
    }

}
