using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DanhSachBenhNhanChoThuNgan;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.YeuCauTiepNhans
{
    public partial interface IYeuCauTiepNhanService
    {
        long HoanUng(long phieuTamUngId);
        decimal GetSoTienHoanUngTrongGoi(long yeuCauTiepNhanId);
        Task LuuTamChiPhiNgoaiTruTrongGoi(QuyetToanDichVuTrongGoiVo chiTienQuyetToan);
        string GetHtmlBangKeNgoaiTruChoThu(ThuPhiKhamChuaBenhVo thuPhiKhamChuaBenhVo);
        string GetHtmlBangKeNgoaiTruTrongGoiChoThu(QuyetToanDichVuTrongGoiVo thuPhiKhamChuaBenhVo);
        bool KiemTraYeuCauTiepNhanCoKhuyenMai(long yeuCauTiepNhanId);
        Task<bool> KiemTraConPhieuThuCongNo(long yeuCauTiepNhanId);
        Task<List<DanhSachDichVuKhuyenMaiBenhNhanVo>> GetDanhSachDichVuKhuyenMaiForGrid(long yeuCauTiepNhanId);
        List<ChiPhiKhamChuaBenhTrongGoiDichVuVo> GetDanhSachDichVuTrongGoiCoBHYTChuaQuyetToan(long yeuCauTiepNhanId);
        int KiemTraDichVuTrongGoiCoBHYT(long yeuCauTiepNhanId);
        Task<KetQuaQuyetToanDichVuTrongGoiCoBHYT> QuyetToanDichVuTrongGoiCoBHYT(QuyetToanDichVuTrongGoiVo chiTienQuyetToan);
        string GetHtmlPhieuChiDichVuTrongGoiCoBHYT(long id, string hostingName);
        
        Task<GridDataSource> GetDanhSachThuPhiNgoaiTruAsync(ThuNganQueryInfo queryInfo, bool isAllData);
        Task<GridDataSource> GetTotalPageDanhSachThuPhiNgoaiTruAsync(ThuNganQueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachChuaThuNgoaiTruAsync(ThuNganQueryInfo queryInfo, bool isAllData);
        Task<GridDataSource> GetTotalPageDanhSachChuaThuNgoaiTruAsync(ThuNganQueryInfo queryInfo);
        Task<GridDataSource> GetDanhSachDaThuNgoaiTruAsync(ThuNganQueryInfo queryInfo, bool isAllData);
        Task<GridDataSource> GetTotalPageDanhSachDaThuNgoaiTruAsync(ThuNganQueryInfo queryInfo);
        void ApDungDichVuKhuyenMai(ApDungKhuyenMaiBenhNhan apDungKhuyenMaiBenhNhan);


        #region Danh Sách Chờ Thu Ngân

        #region tất cả danh sách thu ngân
        Task<GridDataSource> GetDataForGridThuNganAsync(ThuNganQueryInfo queryInfo, bool isAllData);
        //Task<GridDataSource> GetDataForGridDanhSachThuNganAsync(QueryInfo queryInfo, bool isAllData);
        Task<GridDataSource> GetTotalPageForGridDanhSachThuNganAsync(ThuNganQueryInfo queryInfo);
        #endregion     

        List<ChiPhiKhamChuaBenhVo> GetDanhSachChiPhiKhamChuaBenhChuaThu(long yeuCauTiepNhanId);

        string GetHtmlPhieuThuChiNgoaiTru(long id, string hostingName);
        string GetHtmlBaoCaoToaThuoc(long id, string hostingName);
        string GetHtmlBangKeNgoaiTruTrongGoiDv(long yeuCauTiepNhanId, string hostingName, bool xemTruoc = false);
        string GetHtmlBangKeNgoaiTru(long id, string hostingName, bool xemTruoc = false);
        string GetHtmlBangKeNgoaiTruCoBHYT(long id, string hostingName, bool xemTruoc = false);
        string GetHtmlPhieuThuTamUng(long id, string hostingName);
        string GetHtmlPhieuThuBenhNhanTraTien(long id, string hostingName);
        string GetHtmlPhieuChi(long id, string hostingName);

        string GetHtmlPhieuHoanUngNgoaiTru(long taiKhoanChiId, string hostingName);
        string GetHtmlPhieuHoanThuNgoaiTru(long taiKhoanChiId, string hostingName);

        Task<KetQuaThuPhiKhamChuaBenhNgoaiTruVo> ThuPhiKhamChuaBenh(ThuPhiKhamChuaBenhVo thuPhiKhamChuaBenhVo);

        Task<KetQuaThuPhiKhamChuaBenhNgoaiTruVaQuyetToanDichVuTrongGoiVo> ThuPhiKhamChuaBenhVaQuyetToanDichVuTrongGoi(ThuPhiKhamChuaBenhVo thuPhiKhamChuaBenhVo);
        Task<(long, string)> ThuTienTamUng(ThuPhiTamUngVo thuPhiTamUnghVo);
        Task<(long, string)> ThuNo(BenhNhanTraLaiTien benhNhanTraLaiTienVo);
        Task<(long, string)> TraTienBenhNhan(ChiTienLaiBenhNhanVo thuPhiTamUnghVo);
        Task<SoTienTamUngVo> SoTienTamUng(long idYeuCauTiepNhan);
        Task<List<ThongTinPhieuThuVo>> GetSoPhieu(DropDownListRequestModel model, long yeuCauTiepNhanId);
        ThongTinPhieuThu GetThongTinPhieuThu(long soPhieuId, LoaiPhieuThuChiThuNgan loaiPhieu);
        void HuyPhieu(ThongTinHuyPhieuVo thongTinHuyPhieuVo);
        void CapnhatNguoiThuHoiPhieuThu(ThongTinHuyPhieuVo thongTinHuyPhieuVo);
        void ChuyenPhieuThuQuaTamUng(long taiKhoanBenhNhanThuId);
        Task<(long, string)> HoanThuDichVu(ChiPhiKhamChuaBenhVo chiPhiKhamChuaBenhVo);
        Task LuuTamChiPhiNgoaiTru(ThuPhiKhamChuaBenhVo thuPhiKhamChuaBenhVo);
        Task ApDungMiemGiamTuNoiGioiThieu(long yeuCauTiepNhanId);
        Task HuyApDungMiemGiamTuNoiGioiThieu(long yeuCauTiepNhanId);
        #endregion

        #region Danh sách đã thu ngân

        Task<List<ChiPhiDaThanhToanKhamChuaBenhVo>> GetDanhSachChiPhiKhamBenhDaThu(long yeuCauTiepNhanId);

        #endregion

        #region  Danh sach da thu
        Task<GridDataSource> GetTotalPageForGridDanhSachDaThuAsync(QueryInfo queryInfo);
        #endregion

        #region  Thông tin miễm giảm và Thông tin voucher

        ThongTinMienGiamVo GetThongTinMienGiam(long yeuCauTiepNhanId);
        bool ThemThongTinVoucher(long yeuCauTiepNhanId, long benhNhanId, List<long> theVoucherIds);
        Task<bool> IsTrungLoaiVoucher(long[] vouchers);
        //Task<bool> KiemTraSoTienMiemGiam(long yeuCauTiepNhanId, Enums.LoaiMienGiamThem loaiMiemGiam, decimal soTienMG);      
        (bool, string) DeleteVouchers(ThongTinVoucherTheoYeuCauTiepNhan model);
        KetQuaCLS GetThuNganByMaBNVaMaTT(TimKiemThongTinBenhNhan TimKiemThongTinBenhNhan);
        (bool, string) KiemTraThongTinVoucher(string maVocher, long yeuCauTiepNhanId, long benhNhanId);
        (long, string) KiemTraVoucherHopLe(string maVocher);
        Camino.Core.Domain.Entities.Vouchers.Voucher getThongTinVoucher(long voucherId);
        bool kiemTraTheVoucher(string maVoucher);
        List<ThongTinVoucherVo> GetThongTinVouchers(long yeucauTiepNhanId);
        (bool, string) KiemTraTheVoucherSuDung(ThongTinVoucherTheoYeuCauTiepNhan model);

        #endregion

        #region Danh Sách Lịch Sử Thu Ngân
        GridDataSource GetDataForGridDanhSachLichSuThuNganAsync(QueryInfo queryInfo, bool isAllData, long yeuCauTiepNhanId = 0);
        GridDataSource GetTotalPageForGridDanhSachLichSuThuNganNganAsync(QueryInfo queryInfo, long yeuCauTiepNhanId = 0);
        Task<List<ChiPhiKhamChuaBenhVo>> GetDanhSachLichSuDaThu(long taiKhoanBenhNhanThuId);
        Task<ThongTinThanhToanThuChiVo> GetThongTinThanhToanThuChiVo(long taiKhoanBenhNhanThuId, int loaiPhieu);
        Task<List<ChiPhiKhamChuaBenhVo>> GetDanhSachHuyThuNgan(long taiKhoanHuyId);
        Task<GridDataSource> GetDataThuChiTienForThuNganAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataDanhSachDaThuTheoSoHDAsync(QueryInfo queryInfo);
        #endregion

        #region  Phiếu In Tổng hợp
        string GetHtmlPhieuChiPhiKhamBenhCoBHYT(long yeuCauTiepNhanId, string hostingName);
        string GetHtmlPhieuChiPhiKhamBenh(long yeuCauTiepNhanId, string hostingName);
        #endregion

        #region tất cả danh sách đã thu tiền hoàn thành
        Task<GridDataSource> GetDataForGridDanhSachThuNganDaHoanThanhAsync(QueryInfo queryInfo, bool isAllData);
        Task<GridDataSource> GetTotalPageForGridDanhSachThuNganDaHoanThanhAsync(QueryInfo queryInfo);
        #endregion

        #region Kiểm tra yêu khám bệnh có cái nào nhập viện chưa

        Task<bool> KiemTraYeuCauNhapVien(long yeuCauTiepNhanId);
        Task<bool> ChuyenVaoNoiTru(long yeuCauTiepNhanId);
        long? KiemTraSuDungGoi(long yeuCauTiepNhanId);
        bool KiemTraNgoaiTruCoDieuTriNoiTru(long yeuCauTiepNhanId);

        #endregion

        (long, string) KiemTraPhieuThuCoBHYT(long yeuCauYeuCauId);
        (long, string) KiemTraPhieuThuGoiCoBHYT(long yeuCauYeuCauId);
        Task ThayDoiLoaiGiaChuaThu(DoiLoaiGiaDanhSachChiPhiKhamChuaBenh doiLoaiGiaDanhSachChiPhiKhamChuaBenh);
    }
}