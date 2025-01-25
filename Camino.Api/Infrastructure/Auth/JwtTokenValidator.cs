using System.Security.Claims;
using System.Text;
using Camino.Core.DependencyInjection.Attributes;
using Microsoft.IdentityModel.Tokens;

namespace Camino.Api.Infrastructure.Auth
{
    [SingletonDependency(ServiceType = typeof(IJwtTokenValidator))]
    public class JwtTokenValidator : IJwtTokenValidator
    {
        private readonly IJwtTokenHandler _jwtTokenHandler;

        public JwtTokenValidator(IJwtTokenHandler jwtTokenHandler)
        {
            _jwtTokenHandler = jwtTokenHandler;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey, bool checkExpired = false)
        {
            return _jwtTokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ValidateLifetime = checkExpired
            });
        }
    }
}
