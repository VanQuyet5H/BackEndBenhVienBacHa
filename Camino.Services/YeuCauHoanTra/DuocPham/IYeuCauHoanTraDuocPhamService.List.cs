using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.YeuCauHoanTra.DuocPham
{
    public partial interface IYeuCauHoanTraDuocPhamService
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? ycTraThuocId = null, bool forExportExcel = false);

        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);
    }
}
