using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.YeuCauHoanTra.VatTu
{
    public partial interface IYeuCauHoanTraVatTuService
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);
    }
}
