using System.Threading.Tasks;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.GiuongBenhs;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.GiuongBenhs
{
    public interface IGiuongBenhService : IMasterFileService<GiuongBenh>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);

        #region So do giuong benh

        Task<GridDataSource> GetDataForGridSoDoGiuongBenhKhoaAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridSoDoGiuongBenhKhoaAsync(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridSoDoGiuongBenhPhongAsync(QueryInfo queryInfo, long khoaId = 0);
        Task<GridDataSource> GetTotalPageForGridSoDoGiuongBenhPhongAsync(QueryInfo queryInfo, long khoaId = 0);

        Task<GridDataSource> GetDataForGridSoDoGiuongBenhKhoaPhongAsync(QueryInfo queryInfo, long khoaId, long phongId);
        Task<GridDataSource> GetTotalPageForGridSoDoGiuongBenhKhoaPhongAsync(QueryInfo queryInfo, long khoaId, long phongId);

        Task<string> GetTenKhoa(long id);

        Task<string> GetMaTenPhong(long id);

        Task<ResultSoDoPopup> getLstPhongForKhoaPopup(ResultSoDoPopup model);

        #endregion So do giuong benh

        Task<bool> KiemTraMaGiuongBenhAsync(long id, string ma);
        bool GiuongDangCoBenhNhan(long giuongBenhId);
    }
}