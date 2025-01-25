using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Users;

namespace Camino.Services.Users
{
    public interface IUserMessagingTokenService : IMasterFileService<UserMessagingToken>
    {
        Task SetupUserMessagingTokenAsync(long userId, string messagingToken, Enums.DeviceType deviceType);
    }
}
