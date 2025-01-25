using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XetNghiem;
using System.Threading.Tasks;

namespace Camino.Services.XetNghiem
{
    public partial interface IXetNghiemService
    {
        Task<GridDataSource> GetDanhSachYeuCauChayLaiXetNghiemForGridAsync(QueryInfo queryInfo, bool isAllData = false);
        Task<GridDataSource> GetTotalDanhSachYeuCauChayLaiXetNghiemForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachYeuCauChayLaiXetNghiemChiTietForGridAsync(QueryInfo queryInfo, bool isAllData = false);
        Task<GridDataSource> GetTotalDanhSachYeuCauChayLaiXetNghiemChiTietForGridAsync(QueryInfo queryInfo);
        bool TuChoiXetNghiem(TuChoiYeuCauGoiLaiXetNghiem tuChoiYeuCauGoiLaiXetNghiem);
        Task DuyetXetNghiem(DanhSachGoiXetNghiemLai duyetYeuCauGoiLaiXetNghiem);
        ThongTinHanhChinhXN ThongTinHanhChinhXN(long phienXetNghiemId);
        GridDataSource GetDanhSachKQChiTietXetNghiemForGrid(QueryInfo queryInfo);
        GridDataSource GetTotalDanhSachKQChiTietXetNghiemForGrid(QueryInfo queryInfo);
    }
}
