using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DieuTriNoiTru;

namespace Camino.Services.DieuTriNoiTru
{
    public interface INoiTruHoSoKhacService : IMasterFileService<NoiTruHoSoKhac>
    {
        Task<ThongTinHoSoGetInfo> GetNoiTruHoSoKhac(long yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru loaiHoSo);
        

        Task<List<ThongTinHoSoGetInfo>> GetListNoiTruHoSoKhac(long yeuCauTiepNhanId,
            Enums.LoaiHoSoDieuTriNoiTru? loaiHoSo);

        Task<ExistCurrentInfoResultVo> IsThisExistForCuringInfo(long yeuCauTiepNhanId);

        Task<string> PhieuInThongTinDieuTriVaCacDichVu(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams);

        Task<string> PhieuInBienBanCamKetPhauThuat(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams);

        Task<string> PhieuInBienBanHoiChanPhauThuat(BangTheoDoiHoiTinhHttpParamsRequest phieuInBienBanHoiChan);

        Task<string> PhieuInBangKiemAnToanPhauThuat(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams);

        Task<string> PhieuInTomTatHoSoBenhAn(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams);

        Task<string> PhieuInGiayCamKetKyThuatMoi(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams);

        Task<string> PhieuInGiayKhamChuaBenhTheoYc(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams);

        Task<string> PhieuInPhieuKhamGayMeTruocMo(PhieuDieuTriVaServicesHttpParams phieuKhamGayMeTruocMoHttpParams);

        Task<string> PhieuInBangTheoDoiHoiTinh(BangTheoDoiHoiTinhHttpParamsRequest bangTheoDoiHoiTinhHttpParams);
        Task<List<ThongTinHoSoGetInfo>> GetListNoiTruHoSoKhacBangTheoDoi(long yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru? loaiHoSo);

        Task<string> GetChanDoan(long yctnId);

        Task<string> GetNguoiThucHien(long nguoiDangLogin);
        Task<List<LookupItemVo>> GetLoaiHoSoDieuTriNoiTru(DropDownListRequestModel model);
        PhieuBienBanHoiChanPhauThuatGridVo GetThongTinPhieuBienBanHoiChanPhauThuat(long yeuCauTiepNhanId);
        List<DanhSachBienBanHoiChanPhauThuat> DanhSachBienBanHoiPhauThuat(long yeuCauTiepNhanId);
        PhieuBienBanHoiChanPhauThuatGridVo ViewNoiTruHoSoKhac(long noiTruHoSoKhacId);
        Task<List<LookupItemVo>> GetListThongTinThanhVienThamGia(DropDownListRequestModel model);
        Task<List<long>> HoSoKhacIds(long yctn, Enums.LoaiHoSoDieuTriNoiTru loaiNoiTru);
        Task<string> PhieuInGiayCamKetKyThuatMoiHS(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams);
        Task<string> PhieuInGiayThoaThuanLuaChonDichVuKhamTheoYeuCau(PhieuDieuTriVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams);
        string GetChanDoanNhapVien(long yctnId);
        Task<List<LookupItemVo>> GetListNhanViens(DropDownListRequestModel model);
        Task<List<ThongTinHoSoGetInfo>> GetDSNoiTruHoSoKhac(long yeuCauTiepNhanId, Enums.LoaiHoSoDieuTriNoiTru loaiHoSo);
        Task<ThongTinHoSoGetInfo> GetNoiTruHoSoKhacId(long noiTruHoSoKhacId, Enums.LoaiHoSoDieuTriNoiTru loaiHoSo);
        Task<List<DuocPhamTheoKhoThuocTuTrucTheoBenhNhanTemplateVo>> GetListDuocPhamTheoTuTrucKhoaPhongBenhNhanDangNamNoiTru(DropDownListRequestModel model);

        #region giấy phản ứng thuốc BVHD-3874
        Task<string> PhieuINPhanUngThuoc(PhanUngThuocVaServicesHttpParams dieuTriNoiTruVaServicesHttpParams);
        #endregion
    }
}
