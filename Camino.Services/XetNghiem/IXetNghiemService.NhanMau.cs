using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.XetNghiem
{
    public partial interface IXetNghiemService
    {
        Task<GridDataSource> GetDanhSachNhanMauXetNghiemForGrid(QueryInfo queryInfo, bool exportExcel);
        Task<GridDataSource> GetTotalPagesDanhSachNhanMauXetNghiemForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachNhanMauNhomXetNghiemForGrid(QueryInfo queryInfo, bool exportExcel);
        Task<GridDataSource> GetTotalPagesDanhSachNhanMauNhomXetNghiemForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachNhanMauDichVuXetNghiemForGrid(QueryInfo queryInfo, bool exportExcel);
        Task<GridDataSource> GetTotalPagesDanhSachNhanMauDichVuXetNghiemForGrid(QueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachKhongTiepNhanMau(QueryInfo queryInfo);
        Task<int> SoLuongMauCoTheTuChoi(long phieuGoiMauXetNghiemId, long nhomDichVuBenhVienId, long phienXetNghiemId);
        Task TuChoiMau(long mauXetNghiemId, long nhanVienXetKhongDatId, string lyDoTuChoi);
        Task<int> TinhSoLuongMauCoTheTuChoi(long phieuGoiMauId);
    }
}
