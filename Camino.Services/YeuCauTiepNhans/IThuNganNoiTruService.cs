using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.YeuCauTiepNhans
{
    public partial interface IThuNganNoiTruService : IYeuCauTiepNhanBaseService
    {
        long HoanUng(long phieuTamUngId);
        decimal GetSoTienHoanUngTrongGoi(long yeuCauTiepNhanId);
        Task LuuTamChiPhiNoiTruTrongGoi(QuyetToanDichVuTrongGoiVo chiTienQuyetToan);
        string GetHtmlBangKeNoiTruChoThu(ThuPhiKhamChuaBenhNoiTruVo model, string hostingName, out List<ChiPhiKhamChuaBenhNoiTruVo> danhSachTatCaChiPhi);
        string GetHtmlBangKeNoiTruChoThuTrongGoi(QuyetToanDichVuTrongGoiVo model, string hostingName);
        Task<bool> CapNhatDonGiaMoi(CapNhatDonGiaMoi capNhatDonGiaMoi);
        Task<int> KiemTraDichVuTrongGoiCoBHYTNoiTru(long yeuCauTiepNhanId);
        long? KiemTraSuDungGoi(long yeuCauTiepNhanId);
        bool KiemTraYeuCauTiepNhanCoKhuyenMai(long yeuCauTiepNhanId);
        Task<bool> KiemTraConPhieuThuCongNo(long yeuCauTiepNhanId);
        Task<List<DanhSachDichVuKhuyenMaiBenhNhanNoiTruVo>> GetDanhSachDichVuKhuyenMaiForGrid(long yeuCauTiepNhanId);
        void ApDungDichVuKhuyenMai(ApDungKhuyenMaiBenhNhanNoiTru apDungKhuyenMaiBenhNhan);

        //Task<List<ChiPhiKhamChuaBenhTrongGoiDichVuVo>> GetDanhSachDichVuTrongGoiCoBHYTChuaQuyetToanNoiTru(long yeuCauTiepNhanId);
        Task<KetQuaQuyetToanDichVuTrongGoiCoBHYT> QuyetToanDichVuTrongGoiCoBHYTNoiTru(QuyetToanDichVuTrongGoiVo chiTienQuyetToan);

        Task<GridDataSource> GetDanhSachChuaQuyetToanNoiTruAsync(ThuNganNoiTruQueryInfo queryInfo, bool isAllData);
        Task<GridDataSource> GetTotalPageDanhSachChuaQuyetToanNoiTruAsync(ThuNganNoiTruQueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachDaQuyetToanNoiTruAsync(ThuNganNoiTruDaQuyetToanQueryInfo queryInfo, bool isAllData);
        Task<GridDataSource> GetTotalPageDanhSachDaQuyetToanNoiTruAsync(ThuNganNoiTruDaQuyetToanQueryInfo queryInfo);
        Task<List<ChiPhiKhamChuaBenhNoiTruVo>> GetDanhSachChiPhiKhamChuaBenhChuaThu(long yeuCauTiepNhanId);
        Task<decimal> GetSoTienBNConPhaiThanhToan(long yeuCauTiepNhanId);
        Task<List<ThongTinPhieuThuVo>> GetSoPhieu(DropDownListRequestModel model, long yeuCauTiepNhanId);
        ThongTinPhieuThu GetThongTinPhieuThu(long soPhieuId, LoaiPhieuThuChiThuNgan loaiPhieu);
        //Task<decimal> GetSoTienDaTamUngAsync(long yeuCauTiepNhanId);
        Task<(long, string)> ThuTienTamUngNoiTru(ThuPhiTamUngNoiTruVo thuPhiTamUngNoiTruVo);
        Task<KetQuaThuPhiKhamChuaBenhNoiTruVo> ThuPhiNoiTru(ThuPhiKhamChuaBenhNoiTruVo thuPhiKhamChuaBenhVo);
        void HuyPhieu(ThongTinHuyPhieuVo thongTinHuyPhieuVo);
        void CapnhatNguoiThuHoiPhieuThu(ThongTinHuyPhieuVo thongTinHuyPhieuVo);
        Task<(long, string)> HoanThuDichVu(ChiPhiKhamChuaBenhNoiTruVo chiPhiKhamChuaBenhNoiTruVo);
        Task LuuTamChiPhiNoiTru(ThuPhiKhamChuaBenhNoiTruVo thuPhiKhamChuaBenhVo);
        Task ApDungMiemGiamTuNoiGioiThieu(long yeuCauTiepNhanId);
        Task HuyApDungMiemGiamTuNoiGioiThieu(long yeuCauTiepNhanId);
        bool KiemTraTaiKhoanThuCoBHYT(long taiKhoanThuId);
        List<ChiPhiKhamChuaBenhTrongGoiDichVuVo> GetDanhSachDichVuTrongGoiCoBHYTChuaQuyetToanNoiTru(long yeuCauTiepNhanId);
        Task<KetQuaThuPhiKhamChuaBenhNoiTruVaQuyetToanDichVuTrongGoiVo> ThuPhiNoiTruVaQuyetToanDichVuTrongGoi(ThuPhiKhamChuaBenhNoiTruVo thuPhiKhamChuaBenhVo);

        //BVHD-3938 Thêm bảng kê ngoài gói
        string GetHtmlBangKeNgoaiGoiChuaQuyetToan(long yeuCauTiepNhanId, string hostingName, List<ChiPhiKhamChuaBenhNoiTruVo> danhSachTatCaChiPhi);

        Task ThayDoiLoaiGiaChuaThu(DoiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru doiLoaiGiaDanhSachChiPhiKhamChuaBenhNoiTru);

        #region PHIẾU THU CHI , HOÀN ỨNG , BẢNG KÊ.

        string GetHtmlPhieuThuChiNoiTru(long taiKhoanThuId, string hostingName);
        string GetHtmlPhieuHoanUngNoiTru(long taiKhoanThuId, string hostingName);
        string GetHtmlPhieuHoanThuNoiTru(long taiKhoanChiId, string hostingName);
        string GetHtmlBangKeCoBHYT(long taiKhoanThuId, string hostingName, ref TaiKhoanBenhNhanThu taiKhoanBenhNhanThu, ref List<ChiPhiKhamChuaBenhNoiTruVo> tatCaDichVuKhamChuaBenh);
        string GetHtmlBangKe(long taiKhoanThuId, string hostingName, ref TaiKhoanBenhNhanThu taiKhoanBenhNhanThu, ref List<ChiPhiKhamChuaBenhNoiTruVo> tatCaDichVuKhamChuaBenh);
        string GetHtmlBangKeTrongGoiDv(long taiKhoanThuId, string hostingName, ref TaiKhoanBenhNhanThu taiKhoanBenhNhanThu, ref List<ChiPhiKhamChuaBenhNoiTruVo> tatCaDichVuKhamChuaBenh);
        string GetHtmlBangKeCoBHYTChuaQuyetToan(long yeuCauTiepNhanId, string hostingName);
        string GetHtmlBangKeChuaQuyetToan(long yeuCauTiepNhanId, string hostingName);
        string GetHtmlBangKeChuaQuyetToanTrongGoiDv(long yeuCauTiepNhanId, string hostingName);
        
        List<string> GetHtmlBangChuaQuyetToanTheoKhoaChiDinh(long yeuCauTiepNhanId, string hostingName);
        List<string> GetHtmlBangKeTheoKhoaChiDinh(long taiKhoanThuId, string hostingName);

        #endregion

    }
}
