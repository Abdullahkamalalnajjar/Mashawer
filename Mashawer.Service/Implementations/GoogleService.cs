using Google.Apis.Auth;

namespace Mashawer.Service.Implementations
{
    public class GoogleService(UserManager<ApplicationUser> userManger, ApplicationDbContext context, IJwtProvider jwtProvider, IAuthService authService) : IGoogleService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManger;
        private readonly ApplicationDbContext _context = context;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly IAuthService _authService = authService;
        public async Task<AuthResult> GoogleLogin(string idToken, CancellationToken cancellationToken)
        {
            var payload = await VerifyGoogleToken(idToken);
            if (payload == null)
                return AuthResult.Fail("Invalid Google token.");

            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = payload.Email,
                    UserName = payload.Email,
                    FirstName = payload.GivenName ?? "",
                    LastName = payload.FamilyName ?? "",
                    ProfilePictureUrl = payload.Picture,
                    //    PhoneNumber = payload.PhoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return AuthResult.Fail($"Failed to create user: {errors}");
                }

                await _userManager.AddToRoleAsync(user, DefaultRoles.Member);
            }

            var now = DateTime.UtcNow;


            if (user.IsDisable)
                return AuthResult.Fail("Your account has been disabled.");

            var (roles, permissions) = await GetUserRolesAndPermissions(user, cancellationToken);
            var (token, expiresIn) = _jwtProvider.GenerateToken(user, roles, permissions);
            var refreshToken = GeneratedRefreshToken();
            var refreshTokenExpiresIn = now.AddDays(14);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiresIn,
            });

            await _userManager.UpdateAsync(user);

            var response = new AuthResponse(
                Id: user.Id,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Token: token,
                IsDisable: user.IsDisable,
                UserType: user.UserType.ToString(),
                TokenExpiresIn: expiresIn,
                RefreshToken: refreshToken,
                RefreshTokenExpiresIn: refreshTokenExpiresIn
            );

            return AuthResult.Success(response);
        }


        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleToken(string idToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                return payload;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return null;
            }
        }
        private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermissions(ApplicationUser user, CancellationToken cancellationToken)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            //var userPermissions = await _context.Roles
            //    .Join(_context.RoleClaims,
            //        role => role.Id,
            //        claim => claim.RoleId,
            //        (role, claim) => new { role, claim }
            //    )
            //    .Where(x => userRoles.Contains(x.role.Name!))
            //    .Select(x => x.claim.ClaimValue!)
            //    .Distinct()
            //    .ToListAsync(cancellationToken);

            var userPermissions = await (from r in _context.Roles
                                         join p in _context.RoleClaims
                                         on r.Id equals p.RoleId
                                         where userRoles.Contains(r.Name!)
                                         select p.ClaimValue!)
                                         .Distinct()
                                         .ToListAsync(cancellationToken);

            return (userRoles, userPermissions);
        }
        private static string GeneratedRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    }

}
