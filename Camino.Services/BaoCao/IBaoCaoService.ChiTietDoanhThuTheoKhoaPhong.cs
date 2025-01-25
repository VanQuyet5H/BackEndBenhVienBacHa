using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.BaoCaos;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        List<BaoCaoChiTietDoanhThuTheoKhoaPhongGridVo> GetDataForBaoCaoChiTietDoanhThuTheoKhoaPhong(
            DateTimeFilterVo dateTimeFilter);
        Task<GridDataSource> GetBaoCaoChiTietDoanhThuTheoKhoaPhongForExportAsync(BaoCaoChiTietDoanhThuTheoKhoaPhongQueryInfo queryInfo);
        Task<GridDataSource> GetBaoCaoChiTietDoanhThuTheoKhoaPhongForMasterGridAsync(BaoCaoChiTietDoanhThuTheoKhoaPhongQueryInfo queryInfo);
        Task<GridDataSource> GetBaoCaoChiTietDoanhThuTheoKhoaPhongForDetailGridAsync(BaoCaoChiTietDoanhThuTheoKhoaPhongQueryInfo queryInfo);
    }
}
