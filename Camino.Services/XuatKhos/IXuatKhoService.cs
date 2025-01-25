using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;

namespace Camino.Services.XuatKhos
{
    public interface IXuatKhoService : IMasterFileService<XuatKhoDuocPham>
    {
        Task<List<DuocPhamXuatGridVo>> GetDuocPhamOnGroup(long groupId, long khoXuatId, string searchString, List<DaSuaSoLuongXuat> lstDaChon);
        Task<GridDataSource> GetAllDuocPhamData(QueryInfo queryInfo);
        Task<GridDataSource> GetAllDuocPhamTotal(QueryInfo queryInfo);

        Task<GridDataSource> GetDataForGridAsync(QueryInfo queryInfo, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridChildAsync(QueryInfo queryInfo, long? XuatKhoDuocPhamId = null, bool forExportExcel = false);
        Task<GridDataSource> GetTotalPageForGridChildAsync(QueryInfo queryInfo);

        Task<List<LookupItemVo>> GetKhoDuocPham(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetKhoTheoLoaiDuocPham(DropDownListRequestModel model);


        Task<List<LookupItemVo>> GetKhoDuocPhamNhap(DropDownListRequestModel model);
        Task<List<LookupItemVo>> GetKhoLoaiDuocPhamNhap(DropDownListRequestModel model);


        Task<List<LookupItemVo>> GetLoaiXuatKho(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetNguoiXuat(DropDownListRequestModel model);

        Task<List<KhoTemplateVo>> GetListDuocPham(DropDownListRequestModel model);

        Task<List<LookupItemVo>> GetNguoiNhan(DropDownListRequestModel model);

        Task<XuatKhoDuocPhamChiTiet> GetDuocPham(ThemDuocPham model);
        Task<double> GetSoLuongTon(long duocPhamId, bool isDatChatLuong, long khoNhapId);

        Task<decimal> GetDonGiaBan(long duocPhamId);

        Task<XuatKhoDuocPham> UpdateXuatKho(XuatKhoDuocPham entity);
        Task DeleteXuatKho(XuatKhoDuocPham entity);

        Task<bool> IsValidateUpdateOrRemove(long id);
        Task DisabledAutoCommit();
        Task SaveChange();

        Task<bool> IsKhoManagerment(long id);

        Task<bool> IsKhoExists(long id);

        Task<bool> IsKhoLeOrNhaThuoc(long id);

        Task<string> InPhieuXuat(long id, string hostingName);
        Task<long> XuatKhoDuocPham(ThongTinXuatKhoDuocPhamVo thongTinXuatKhoDuocPhamVo);

        #region other

        Task<NhapKhoDuocPhamChiTiet> GetNhapKhoDuocPhamChiTietById(long id);

        #endregion other
    }
}