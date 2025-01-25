using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.NguoiGioiThieu;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.NoiGioiThieu;

namespace Camino.Services.NoiGioiThieu
{
    public interface INoiGioiThieuService : IMasterFileService<Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu>
    {
        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridDonViMauAsync(QueryInfo queryInfo, bool exportExcel = false);
        Task<GridDataSource> GetTotalPageForGridDonViMauAsync(QueryInfo queryInfo);
        Task<List<NguoiQuanLyTemplateVo>> GetNguoiQuanLyListAsync(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetDonViMaus(DropDownListRequestModel queryInfo);
        Task<bool> IsTenExists(string ten = null, long Id = 0);
        Task<bool> IsPhoneNumberExists(string ten = null, string soDienThoai = null, long Id = 0);
        Task<bool> IsMaTenExists(string maHoacTen = null, long Id = 0, bool flag = false);

        #region BVHD-3882
        Task GetDonGia(ThongTinGiaVo thongTinDichVu);
        Task<string> GetTenNhomGiaTheoLoaiDichVuAsync(NoiGioiThieuChiTietMienGiam noiGioiThieuChiTietMienGiam);
        Task<GridDataSource> GetDataForGridChiTietMienGiamAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridChiTietMienGiamAsync(QueryInfo queryInfo);
        Task<List<DichVuKyThuatTemplateVo>> GetDichVuKyThuat(DropDownListRequestModel model);
        Task<List<DichVuKyThuatTemplateVo>> GetDichVuGiuong(DropDownListRequestModel model);
        Task<List<DichVuKyThuatTemplateVo>> GetDichVuKham(DropDownListRequestModel model);
        Task<List<DichVuKyThuatTemplateVo>> GetDuocPham(DropDownListRequestModel model);
        Task<List<DichVuKyThuatTemplateVo>> GetVatTu(DropDownListRequestModel model);

        Task XuLyThemMienGiamDichVuAsync(NoiGioiThieuChiTietMienGiam noiGioiThieuChiTietMienGiam);
        Task<NoiGioiThieuChiTietMienGiam> XuLyGetMienGiamDichVuAsync(long id);
        Task XuLyCapNhatMienGiamDichVuAsync(NoiGioiThieuChiTietMienGiam noiGioiThieuChiTietMienGiam);
        Task XuLyXoaMienGiamDichVuAsync(long id);
        #endregion


        #region BVHD-3936
        Task<NoiGioiThieuDataImportVo> XuLyKiemTraDataDichVuMienGiamImportAsync(NoiGioiThieuFileImportVo info);
        Task KiemTraDataDichVuMienGiamImportAsync(List<ThongTinDichVuMienGiamTuFileExcelVo> datas, NoiGioiThieuDataImportVo result, long? noiGioiThieuId);
        Task<List<NoiGioiThieuChiTietMienGiamGridVo>> XuLyLuuDichVuMienGiamImportAsync(List<ThongTinDichVuMienGiamTuFileExcelVo> datas, long? noiGioiThieuId);
        #endregion
    }
}
