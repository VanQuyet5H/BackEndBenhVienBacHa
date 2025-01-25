using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.XetNghiem
{
    public partial interface IXetNghiemService
    {
        Task<GridDataSource> GetDanhSachGoiMauXetNghiemForGrid(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPagesDanhSachGoiMauXetNghiemForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachGoiMauNhomXetNghiemForGrid(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPagesDanhSachGoiMauNhomXetNghiemForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachGoiMauDichVuXetNghiemForGrid(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPagesDanhSachGoiMauDichVuXetNghiemForGrid(QueryInfo queryInfo);
        Task<string> TinhSoLuongMauGoiMauXetNghiem(QueryInfo queryInfo, long phieuGoiMauId);
        Task<int> TongSoLuongMauGoiMauXetNghiem(QueryInfo queryInfo, long phieuGoiMauId);
        Task<int> SoLuongMauDaHoanThanhGoiMauXetNghiem(QueryInfo queryInfo, long phieuGoiMauId);
        Task XoaPhieuGoiMauXetNghiem(long phieuGoiMauId);
        Task<PhieuGoiMauXetNghiem> GetPhieuGoiMauXetNghiem(long phieuGoiMauId);
        #region grid search popup in xet nghiem
        Task<List<DuyetKqXetNghiemChiTietGridVo>> GetDanhSachSearchPopupForGrid(TimKiemPopupInXetNghiemVo model);
        #endregion
    }
}