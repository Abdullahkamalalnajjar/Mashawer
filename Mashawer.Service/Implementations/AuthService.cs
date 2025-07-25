using Microsoft.AspNetCore.WebUtilities;

namespace Mashawer.Service.Implementations
{
    public class AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _context = context;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly int _tokenExpiresIn = 14; // 14 days
        public async Task<AuthResult> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return AuthResult.Fail("Incorrect email or password. Please try again.");

            var now = DateTime.UtcNow;

            //// Email confirmation and account status
            //if (!user.EmailConfirmed)
            //    return AuthResult.Fail("Email not confirmed. Please verify your email.");

            if (user.IsDisable)
                return AuthResult.Fail("Your account has been disabled.");

            if (user.LockoutEnd.HasValue && user.LockoutEnd > now)
                return AuthResult.Fail("Your account is temporarily locked. Please try again later.");

            // Password check
            var signInResult = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: true);
            if (!signInResult.Succeeded)
                return AuthResult.Fail("Incorrect email or password. Please try again.");

            var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);
            var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);
            var refreshToken = GeneratedRefreshToken();
            var refreshTokenExpiresIn = now.AddDays(_tokenExpiresIn);

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
                TokenExpiresIn: expiresIn,
                RefreshToken: refreshToken,
                UserType: user.UserType.ToString(),
                AgentAddress: user.AgentAddress,
                RepresentativeAddress: user.RepresentativeAddress,
                RefreshTokenExpiresIn: refreshTokenExpiresIn
            );

            return AuthResult.Success(response);
        }




        public async Task<AuthResponse> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = _jwtProvider.ValidateToken(token);
            if (string.IsNullOrEmpty(userId))
                return null;
            var user = await _context.Users.Include(r => r.RefreshTokens).Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
            if (user is null)
                return null;
            var refreshTokenEntity = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
            if (refreshTokenEntity is null)
                return null;
            // make the refresh token revoke
            refreshTokenEntity.RevokedOn = DateTime.UtcNow;
            // Generate the new token
            var (userRoles, userPerimission) = await GetUserRolesAndPermissions(user, cancellationToken);
            var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPerimission);
            // Generate the new refresh token
            var newRefreshToken = GeneratedRefreshToken();
            var newRefreshTokenExpiresIn = DateTime.UtcNow.AddDays(_tokenExpiresIn);
            // add the refresh token in the user
            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresOn = newRefreshTokenExpiresIn,
            });
            // update the user
            await _userManager.UpdateAsync(user);
            // return the response
            var response = new AuthResponse(Id: user.Id,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Token: newToken,
                IsDisable: user.IsDisable,
                UserType: user.UserType.ToString(),
                AgentAddress: user.AgentAddress,
                RepresentativeAddress: user.RepresentativeAddress,
                TokenExpiresIn: expiresIn,
                RefreshToken: newRefreshToken,
                RefreshTokenExpiresIn: newRefreshTokenExpiresIn
                );
            return response;

        }

        public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {

            var userId = _jwtProvider.ValidateToken(token);
            if (string.IsNullOrEmpty(userId))
                return false;
            var user = await _context.Users.Include(r => r.RefreshTokens).Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
            if (user is null)
                return false;
            var refreshTokenEntity = user.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken && x.IsActive);
            if (refreshTokenEntity is null)
                return false;
            // make the refresh token revoke
            refreshTokenEntity.RevokedOn = DateTime.UtcNow;
            // update the user
            await _userManager.UpdateAsync(user);
            // return the response  
            return true;
        }
        private static string GeneratedRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        public async Task<string> ConfirmEmailAsync(string userId, string code)
        {
            if (await _userManager.FindByIdAsync(userId) is not { } user)
                return "InvalidCode";
            if (user.EmailConfirmed)
                return "Email already confirmed";
            var decodedCode = code;

            try
            {
                decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            }
            catch (FormatException)
            {

                return "InvalidCode";
            }
            var result = await _userManager.ConfirmEmailAsync(user, decodedCode);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, DefaultRoles.Member);
                return "Email confirmed successfully";
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return errors;
            }
        }

        public async Task<string> ResendConfirmEmailAsync(string email)
        {
            if (await _userManager.FindByEmailAsync(email) is not { } user)
                return "Good";
            if (user.EmailConfirmed)
                return "Daplicated Confirmation";
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            return "Code Has been resend";
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
    }

}
