using System.Threading.Tasks;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.DuyetPhieuTraVatTuTuBns
{
    public interface IDuyetPhieuTraVatTuTuBnService : IMasterFileService<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);

        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsyncVatTuChild(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsyncBenhNhanChild(QueryInfo queryInfo);

        Task<double> GetSoLuongXuat(long vatTuBenhVienId, bool laVatTuBHYT, long? hopDongThauVatTuId, long khoId);
    }
}
