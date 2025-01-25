using System.Threading.Tasks;
using Camino.Core.Domain.Entities.CongTyUuDais;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.CongTyUuDais
{
    public interface ICongTyUuDaiService : IMasterFileService<CongTyUuDai>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<bool> IsTenExists(string ten = null, long id = 0);
    }
}
