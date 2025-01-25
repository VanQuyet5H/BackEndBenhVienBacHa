using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Camino.Api.Auth;
using Camino.Api.Models.Auth;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Microsoft.Extensions.Options;

namespace Camino.Api.Infrastructure.Auth
{
    [SingletonDependency(ServiceType = typeof(IJwtFactory))]
    public class JwtFactory : IJwtFactory
    {
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtFactory(IJwtTokenHandler jwtTokenHandler, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtTokenHandler = jwtTokenHandler;
            _jwtOptions = jwtOptions.Value;
        }

        public AccessToken GenerateInternalToken(long id, params long[] roleId)
        {
            var claims = new[]
            {
                new Claim(Constants.JwtClaimTypes.Id, id.ToString()),
                new Claim(Constants.JwtClaimTypes.Role, string.Join(Constants.JwtRoleSeparator, roleId))
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                _jwtOptions.InternalIssuer,
                _jwtOptions.Audience,
                claims,
                _jwtOptions.NotBefore,
                _jwtOptions.Expiration,
                _jwtOptions.SigningCredentials);

            return new AccessToken(id,_jwtTokenHandler.WriteToken(jwt), (int)_jwtOptions.ValidFor.TotalSeconds);
        }
        public AccessToken GeneratePortalToken(long id)
        {
            var claims = new[]
            {
                new Claim(Constants.JwtClaimTypes.Id, id.ToString())
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                _jwtOptions.PortalIssuer,
                _jwtOptions.Audience,
                claims,
                _jwtOptions.NotBefore,
                _jwtOptions.Expiration,
                _jwtOptions.SigningCredentials);

            return new AccessToken(id, _jwtTokenHandler.WriteToken(jwt), (int)_jwtOptions.ValidFor.TotalSeconds);
        }
    }
}
