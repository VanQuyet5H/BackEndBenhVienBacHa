using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.YeuCauHoanTra.KSNK
{
    public partial interface IYeuCauHoanTraKSNKService
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridDuocPhamChildAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridDuocPhamChildAsync(QueryInfo queryInfo);

    }
}
