using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauHoanTra.KSNK
{
    public partial interface IYeuCauHoanTraKSNKService
    {
        Task<GridDataSource> GetDanhSachHoanTraKSNKForGridAsync(QueryInfo queryInfo, bool isAllData = false);
        Task<GridDataSource> GetTotalDanhSachHoanTraKSNKForGridAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDanhSachHoanTraKSNKChiTietForGridAsync(QueryInfo queryInfo, bool isAllData = false);
        Task<GridDataSource> GetTotalDanhSachHoanTraKSNKChiTietForGridAsync(QueryInfo queryInfo);

        Task<ThongTinHoanTraKSNK> GetThongTinHoanTraKSNK(long yeuCauHoanTraVatTuId);
        Task DuyetHoanTraNhapKho(long id, long nguoiNhanId, long nguoiXuatId);
        Task DuyetHoanTraDuocPham(long yeuCauTraDuocPhamId, long nhanVienNhanId, long nhanVienTraId);
        Task TuChoiDuyetHoanTraKSNK(long id, string lyDoHuy);
        Task TuChoiDuyetHoanTraDuocPhamKSNK(long yeuCauTraDuocPhamId, string lyDoKhongDuyet);
        Task<ThongTinHoanTraKSNK> GetThongTinDuyetHoanTraDuocPham(long yeuCauHoanTraDuocPhamId);
        string GetHtmlPhieuInHoanTraKSNK(long yeuCauHoanTraVatTuId, string hostingName);
        Task<GridDataSource> GetDanhSachDuyetHoanTraDuocPhamChiTietForGridAsync(QueryInfo queryInfo, bool isAllData = false);
    }
}