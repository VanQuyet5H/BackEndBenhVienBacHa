using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhoVatTus;
using System.Threading.Tasks;

namespace Camino.Services.NhapKhoVatTus
{
    public partial interface INhapKhoVatTuService
    {
        Task<GridDataSource> GetDanhSachHoanTraVatTuForGridAsync(QueryInfo queryInfo, bool isAllData = false);
        Task<GridDataSource> GetTotalDanhSachHoanTraVatTuForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachHoanTraVatTuChiTietForGridAsync(QueryInfo queryInfo, bool isAllData = false);
        Task<GridDataSource> GetTotalDanhSachHoanTraVatTuChiTietForGridAsync(QueryInfo queryInfo);
        Task<ThongTinHoanTraVatTu> GetThongTinHoanTraVatTu(long yeuCauHoanTraVatTuId);
        Task DuyetHoanTraNhapKho(long id, long nguoiNhanId, long nguoiXuatId);
        Task TuChoiDuyetHoanTraVatTu(long id, string lyDoHuy);
        string GetHtmlPhieuInHoanTraVatTu(long yeuCauHoanTraVatTuId, string hostingName);
    }
}