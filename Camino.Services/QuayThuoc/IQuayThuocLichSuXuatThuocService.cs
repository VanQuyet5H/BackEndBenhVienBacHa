using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.QuayThuoc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Services.QuayThuoc
{
    public interface IQuayThuocLichSuXuatThuocService : IMasterFileService<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien>
    {
        void HuyBanThuocTrongNgay(ThongTinHuyPhieuXuatTrongNgayVo huyBanThuocTrongNgayViewModel);

        #region  DANH SÁCH LỊCH SỬ XUẤT THUỐC 07/09/2020

        Task<GridDataSource> GetDanhSachLichSuXuatThuoc(QueryInfo queryInfo, bool isPrint);
        Task<GridDataSource> GetTotalDanhSachLichSuXuatThuoc(QueryInfo queryInfo);
        List<ThongTinBenhNhanGridVo> GetThongTinBenhNhanTheoMaBN(string maBN);
        List<ThongTinDuocPhamQuayThuocVo> GetDanhSachLichSuXuatThuocVatTuBHYT(long phieuXuatId, LoaiDuocPhamVatTu loaiDuocPhamVatTu);
        List<ThongTinDuocPhamQuayThuocVo> GetDanhSachLichSuXuatThuocVatTukhongBHYT(long phieuXuatId, LoaiDuocPhamVatTu loaiDuocPhamVatTu);
        Task<string> XacNhanInThuocVatTuCoBhytXuatThuoc(XacNhanInThuocVatTu xacNhanIn, bool coBhyt);

        #endregion


        // lịch sử xuất thuốc của quầy thuốc (không BHYT)
        Task<GridDataSource> GetDataForGridLichSuXuatThuocAsync(QueryInfo queryInfo, bool isPrint);
        //Task<GridDataSource> GetTotalPageForGridLichSuXuatThuocAsync(QueryInfo queryInfo);
        List<ThongTinBenhNhanGridVo> GetThongTinBenhNhanDetail(long ycTiepNhanId, string idBenhNhan);
        Task<List<ThongTinDuocPhamQuayThuocVo>> GetDanhSachThuocDaXuatThuocBHYTLS(long tiepNhanId, long? idTaiKhoanBenhNhanThu);
        Task<List<ThongTinDuocPhamQuayThuocVo>> GetDanhSachThuocDaXuatThuocKhongBHYTLS(long tiepNhanId, long? idTaiKhoanBenhNhanThu);
        // lịch sử xuất thuốc (BHYT và không BHYT)
        Task<GridDataSource> GetDataForGridLichSuXuatThuoc(QueryInfo queryInfo, bool isPrint);
        Task<GridDataSource> GetTotalPageForGridLichSuXuatThuoc(QueryInfo queryInfo);

        #region BVHD-3941

        Task<long> GetThongTinCongTyBaoHiemTuNhanTheoMaTN(string maTN);


        #endregion
    }
}
