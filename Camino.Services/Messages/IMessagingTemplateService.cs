using System.Threading.Tasks;
using Camino.Core.Domain.Entities.Messages;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.Messages
{
    public interface IMessagingTemplateService : IMasterFileService<MessagingTemplate>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
    }
}
