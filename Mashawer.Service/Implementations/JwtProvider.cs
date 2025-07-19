

using System.Text.Json;

namespace Mashawer.Service.Implementations
{
    public class JwtProvider(IOptions<JwtSettings> options) : IJwtProvider
    {
        private readonly JwtSettings _options = options.Value;

        public (string token, int expiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions)
        {
            // Implementation for generating JWT token

            // make sure to include user claims, expiration time, and signing credentials
            Claim[] UserClaims = [
                new (JwtRegisteredClaimNames.Sub, user.Id),
                new (JwtRegisteredClaimNames.Email, user.Email!),
                new (JwtRegisteredClaimNames.GivenName, user.FirstName),
                new (JwtRegisteredClaimNames.FamilyName, user.FullName),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(nameof(roles), JsonSerializer.Serialize(roles), JsonClaimValueTypes.JsonArray),
            new(nameof(permissions), JsonSerializer.Serialize(permissions), JsonClaimValueTypes.JsonArray)

            ];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(1);
            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: UserClaims,
                expires: DateTime.UtcNow.AddDays(_options.AccessTokenExpireDate),
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return (tokenString, (int)expires.Subtract(DateTime.UtcNow).TotalSeconds);

        }

        public string ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret)),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                var claimsPrincipal = handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                return userId;
            }
            catch (SecurityTokenExpiredException)
            {
                return null;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }


}
