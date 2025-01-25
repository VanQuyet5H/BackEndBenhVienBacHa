using Camino.Api.Models.Auth;

namespace Camino.Api.Infrastructure.Auth
{
    public interface IJwtFactory
    {
        AccessToken GenerateInternalToken(long id, params long[] roleId);
        AccessToken GeneratePortalToken(long id);
    }
}
