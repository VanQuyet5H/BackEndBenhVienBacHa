using System.Security.Claims;

namespace Camino.Api.Infrastructure.Auth
{
    public interface IJwtTokenValidator
    {
        ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey, bool checkExpired = false);
    }
}
