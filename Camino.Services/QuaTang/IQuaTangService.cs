using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.QuaTang
{
    public interface IQuaTangService : IMasterFileService<Core.Domain.Entities.QuaTangs.QuaTang>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<bool> IsTenExists(string tenQuaTang = null, long quaTangId = 0);

    }
}
