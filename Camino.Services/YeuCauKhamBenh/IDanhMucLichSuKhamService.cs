
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial interface IYeuCauKhamBenhService
    {
        Task<GridDataSource> GetDataForGridAsyncDanhMucLichSuKhamBenh(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsyncDanhMucLichSuKhamBenh(QueryInfo queryInfo);
        Task<ThongTinYeuCauKhamVo> GetThongTinYeuCauKham(long yeuCauKhamBenhId);
        Task<ThongTinBenhNhanVo> GetThongTinBenhNhan(long yeuCauKhamBenhId);


    }
}
