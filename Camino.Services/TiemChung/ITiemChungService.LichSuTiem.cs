using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.TiemChung
{
    public partial interface ITiemChungService
    {
        Task<GridDataSource> GetDataForGridLichSuTiemChungAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridLichSuTiemChungAsync(QueryInfo queryInfo);
    }
}
