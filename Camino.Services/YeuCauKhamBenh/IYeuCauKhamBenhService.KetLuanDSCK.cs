using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.DichVuKyThuat;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.ValueObject.YeuCauKhamBenh;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial interface IYeuCauKhamBenhService
    {
        GridDataSource GetDataForGridKeToa(QueryInfo queryInfo);
        GridDataSource GetTotalPageForGridKeToa(QueryInfo queryInfo);
        Task<List<ICDTemplateVo>> GetICDs(DropDownListRequestModel queryInfo);
        Task<List<ICDKhacsTemplateVo>> GetICDKhacs(DropDownListRequestModel queryInfo);

        Task<List<NhanVienHoTongTemplateVo>> GetNhanVienHoTongs(DropDownListRequestModel queryInfo);
        Task<List<LookupItemTemplateVo>> GetNoiThucHiens(DropDownListRequestModel queryInfo, string selectedItems = null);
        Task<List<LookupItemVo>> GetBacSiKhams(DropDownListRequestModel queryInfo);
        Task<List<DichVuKyThuatTemplateVo>> GetDichVuKyThuatBenhViens(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetLoaiGia();
        Task<List<LookupItemVo>> LoaiGiaHieuLucTheoDichVuKham(DropDownListRequestModel model);
        Task<List<LookupItemVo>> LoaiGiaHieuLucTheoDichVuKyThuat(DropDownListRequestModel model);
        Task<List<LookupItemVo>> LoaiGiaHieuLucTheoDichVuGiuong(DropDownListRequestModel model);
        Task<string> GetMucDoDiUng(MucDoDiUngThuocVo mucDoVo);
        Task<List<LookupItemVo>> GetKhoaPhongNhapViens(DropDownListRequestModel queryInfo);
        Task<List<LookupItemVo>> GetBenhVienChuyenViens(DropDownListRequestModel queryInfo);
        Task<GridDataSource> GetDataForGridAsyncToaThuocMau(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncToaThuocMau(QueryInfo queryInfo);
        GridDataSource GetDataForGridToaThuocMauChiTietChild(QueryInfo queryInfo);
        GridDataSource GetTotalPageForToaThuocMauChiTietChild(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridAsyncLichSuKeToa(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridAsyncLichSuKeToa(QueryInfo queryInfo);
        GridDataSource GetDataForGridLichSuKeToaChild(QueryInfo queryInfo);
        GridDataSource GetTotalPageForLichSuKeToaChild(QueryInfo queryInfo);
        Task<List<MaDichVuTemplateVo>> GetDichVuKhamBenh(DropDownListRequestModel model);
        Task<bool> KiemTraCoDonThuoc(long yeuCauKhamBenhId);
        Task<bool> KiemTraCoBHYT(long yeuTiepNhanId);
        Task<KiemTraCoBHYTDuocHuongBaoHiem> KiemTraDeChonLoaiThuoc(long yeuCauTiepNhanId, long yeuCauKhamBenhId);
        GetDuocPhamTonKhoGridVoItem GetDuocPhamInfoById(ThongTinThuocVo thongTinThuocVo);
        VatTuTrongKhoVo GetVatTuInfoById(ThongTinVatTuVo thongTinThuocVo);

        //BenhVienGiaBaoHiem GiaBenhVien(long nhomGiaDichVuKhamBenhBenhVienId, long dichVuKhamBenhBenhVienId);
        Task<ThongTinDichVuKhamTiepTheo> GiaBenhVienAsync(ThongTinDichVuKhamTiepTheo thongTinDichVuKhamTiepTheo);
        List<string> GetThoiGianDonThuoc();
        Task<List<string>> GetGhiChuDonThuocChiTietString(DropDownListRequestModel queryInfo);
        Task<List<string>> GetLyDoNhapVienString(DropDownListRequestModel queryInfo);

        List<string> GetGhiChuDonThuocChiTiet();
        //Task<List<DuocPhamTemplate>> GetDuocPhamKeToaAsync(DropDownListRequestModel queryInfo);
        Task<List<DuocPhamVaVatTuTemplate>> GetDuocPhamVaVatTuKeToaAsync(DropDownListRequestModel queryInfo, bool laNoiTruDuocPham = false);
        //string InDonThuocKhamBenh(long yeuCauKhamBenhId, string hostingName, bool header);
        string InDonThuocKhamBenh(InToaThuocReOrder inToaThuoc);
        Task<string> ThemDonThuocChiTiet(DonThuocChiTietVo donThuocChiTiet);
        Task<string> CapNhatDonThuocChiTiet(DonThuocChiTietVo donThuocChiTiet);
        Task<string> XoaDonThuocChiTiet(DonThuocChiTietVo donThuocChiTiet);
        Task<string> XoaDonThuocTheoYeuCauKhamBenh(XoaDonThuocTheoYeuCauKhamBenhVo xoaDonThuocTheoYeuCauKhamBenh);
        Task<string> TangHoacGiamSTTDonThuocChiTiet(DonThuocChiTietTangGiamSTTVo donThuocChiTiet);

        Task<bool> KiemTraDonThuocDaXuatHayDaThanhToan(XoaDonThuocTheoYeuCauKhamBenhVo xoaDonThuocTheoYeuCauKhamBenh);

        Task<KetQuaApDungToaThuocVo> ApDungToaThuocLichSuKhamBenhAsync(ApDungToaThuocLichSuKhamBenhVo apDungToaThuocLichSuKhamBenhVo);
        Task<string> ApDungToaThuocLichSuKhamBenhConfirmAsync(ApDungToaThuocLichSuKhamBenhConfirmVo apDungToaThuocLichSuKhamBenhConfirmVo);
        Task<KetQuaApDungToaThuocVo> ApDungToaMauAsync(ApDungToaThuocMauVo toaMauVo);
        Task<string> ApDungToaThuocConfirmAsync(ApDungToaThuocConfirmVo apDungToaThuocConfirmVo);
        Task<GridDataSource> GetChanDoanBacSiKhacDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetChanDoanBacSiKhacTotalPageForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetICDKhacDataForGridAsyncDetail(QueryInfo queryInfo);
        Task<GridDataSource> GetICDKhacTotalPageForGridAsyncDetail(QueryInfo queryInfo);
        Task<GridDataSource> GetDonThuocBacSiKhacDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDonThuocBacSiKhacTotalPageForGridAsync(QueryInfo queryInfo);
        GridDataSource GetDonThuocBacSiKhacDataForGridAsyncDetail(QueryInfo queryInfo);
        GridDataSource GetDonThuocBacSiKhacTotalPageForGridAsyncDetail(QueryInfo queryInfo);
        string FormatNumber(double? inputNumber);
        bool KiemTraCoThoiDiemBatDauDieuTri(long? yeuCauDichVuKyThuatId);
        Task<bool> KiemTraNgayChuyenVien(DateTime? ngayChuyenVien, long yeuCauKhamBenhId);
        Task<bool> KiemTraChiDinhDichVuDaThemTheoYeuCauTiepNhanAsync(long yeuCauTipeNhanId, long dichVuBenhVienId, Enums.EnumNhomGoiDichVu? nhomDichVu);
        Task<bool> KiemTraChiDinhGoiDichVuDaCoDichVuTheoYeuCauTiepNhanAsync(long yeuCauTiepNhanId, long goiDichVuId, List<string> lstDichVu);
        Task<bool> KiemTraChiDinhDichVuKyThuatDaCoTheoYeuCauTiepNhanAsync(long yeuCauTiepNhanId, List<string> lstDichVu, long? noiTruPhieuDieuTriId = null);
        Task ThemGhiChuDonThuocHoacVatTuChiTiet(InputStringStoredVo inputStringStoredVo);
        Task<bool> CheckDonThuocChiTietExist(long donThuocChiTietId);
        Task<bool> CheckVatTuChiTietExist(long vatTuChiTietId);

        Task<bool> KiemTraNhomDichVuChiDinhDichVuKyThuat(long? nhomDichVuId, List<string> lstDichVu);
        Task<string> KiemTraDonThuocChiTietThanhToan(long donThuocChiTietId);
        Task<string> KiemTraVatTuChiTietThanhToan(long yeuCauKhamBenhDonVTYTId);

        Task<bool> KiemTraNhomDichVuDungTheoDichVuChiDinhDichVuKyThuat(long? nhomDichVuId, List<string> lstDichVu);
        Task<string> ThemVatTuChiTiet(VatTuChiTietVo vatTuChiTiet);
        Task<string> CapNhatVatTuChiTiet(VatTuChiTietVo vatTuChiTiet);
        Task<string> XoaVatTuChiTiet(VatTuChiTietVo vatTuChiTiet);
        Task<GridDataSource> GetVatTuYTDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetVatTuYTTotalPageForGridAsync(QueryInfo queryInfo);
        string InVatTuKhamBenh(InVatTuReOrder inVatTu);
        string InToaThuocKhamBenhDanhSach(InToaThuocKhamBenhDanhSach inToaThuocDanhSach);
        Task<string> GetLoiDanTheoICD(long iCDId);
        Task<string> KiemTraMucTranChiPhi(long yeuCauKhamBenhId);
        Task<string> GetSoDangKyDuocPhamNgoaiBv();
        Task<bool> IsTenICDKhacExists(long? idICD, long id, long yeuCauKhamBenhId);
        Task<string> GetTenICD(long id);
        Task<bool> LaBacSiKeDon(KiemTraThuocTrungBSKe kiemTraThuocTrungBSKe);
        string FormatTenDuocPham(string tenThuongMai, string tenQuocTe, string hamLuong, long? duocPhamBenhVienPhanNhomId, bool? laPhieuThucHienThuoc = null);
        string FormatSoLuong(double soLuong, Enums.LoaiThuocTheoQuanLy? loaiThuoc);
        string GetPhuongPhapKyThuatDieuTri(long yeuCauKhamBenhId);

        #region In dịch thuốc BHYT và Khong BHYT của bệnh nhân ds lịch sử khám bệnh
        string InToaThuocBHYTVaKhongBHYTDanhSachKhamBenh(InToaThuocKhamBenhDanhSach inToaThuoc);
        #endregion

        #region BVHD-3575
        Task<bool> KiemTraChiDinhDichVuKhamBenhDaCoTheoYeuCauTiepNhanAsync(long yeuCauTiepNhanId, long dichVuKhamBenhVienId, long noiTruPhieuDieuTriId);


        #endregion
    }
}
