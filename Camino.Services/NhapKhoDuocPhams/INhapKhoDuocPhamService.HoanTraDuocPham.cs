using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoDuocPhamGridVo;
using Camino.Core.Domain.ValueObject.KhoDuocPhams;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.NhapKhoDuocPhams
{
    public partial interface INhapKhoDuocPhamService
    {
        Task<GridDataSource> GetDanhSachDuyetHoanTraDuocPhamForGridAsync(QueryInfo queryInfo, bool isAllData = false);
        Task<GridDataSource> GetTotalDanhSachDuyetHoanTraDuocPhamForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync(QueryInfo queryInfo, bool isAllData = false);
        Task<GridDataSource> GetTotalDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync(QueryInfo queryInfo);
        Task<ThongTinDuyetHoanTraDuocPham> GetThongTinDuyetHoanTraDuocPham(long yeuCauHoanTraDuocPhamId);
        Task DuyetHoanTraDuocPham(long yeuCauTraDuocPhamId, long nhanVienTraId, long nhanVienNhanId, string tenNhanVienNhan, string tenNhanVienTra);
        Task TuChoiDuyetHoanTraDuocPham(long yeuCauTraDuocPhamId, string lyDoKhongDuyet);
        string GetHtmlPhieuInHoanTraDuocPham(long yeuCauHoanTraDuocPhamId, string hostingName);
        string InPhieuHoanTraDuocPhamVatTu(PhieuHoanTraDuocPhamVatTu phieuHoanTraDuocPhamVatTu);
        string InPhieuHoanTraDuocPhamVatTuUpdate(PhieuHoanTraDuocPhamVatTu phieuHoanTraDuocPhamVatTu);

    }
}