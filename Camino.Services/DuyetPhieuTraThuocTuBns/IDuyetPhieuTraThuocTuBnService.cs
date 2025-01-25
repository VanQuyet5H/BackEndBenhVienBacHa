using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.DuyetPhieuTraThuocTuBns
{
    public interface IDuyetPhieuTraThuocTuBnService : IMasterFileService<YeuCauTraDuocPhamTuBenhNhan>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsyncDuocPhamChild(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsyncBenhNhanChild(QueryInfo queryInfo);

        Task<double> GetSoLuongXuat(long duocPhamBenhVienId, bool laDuocPhamBHYT, long? hopDongThauDuocPhamId, long khoId);
    }
}
