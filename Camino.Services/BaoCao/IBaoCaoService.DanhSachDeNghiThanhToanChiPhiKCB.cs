using Camino.Core.Domain.ValueObject.DanhSachDeNghiThanhToanChiPhiKCB;
using Camino.Core.Domain.ValueObject.Grid;
using System.Threading.Tasks;

namespace Camino.Services.BaoCao
{
    public partial interface IBaoCaoService
    {
        Task<GridDataSource> GetDanhSachDeNghiThanhToanChiPhiKCBNgoaiTruForGrid(DanhSachDeNghiThanhToanChiPhiKCBQueryInfoVo queryInfo);
        Task<GridDataSource> GetDanhSachDeNghiThanhToanChiPhiKCBNoiTruForGrid(DanhSachDeNghiThanhToanChiPhiKCBQueryInfoVo queryInfo);

        byte[] ExportDanhSachDeNghiThanhToanChiPhiKCBNgoaiTru(GridDataSource gridDataSource, DanhSachDeNghiThanhToanChiPhiKCBQueryInfoVo query);
        byte[] ExportDanhSachDeNghiThanhToanChiPhiKCBNoiTru(GridDataSource gridDataSource, DanhSachDeNghiThanhToanChiPhiKCBQueryInfoVo query);
    }
}