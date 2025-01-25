using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class PhauThuatThuThuatGridVo : GridItem
    {
        public int SoThuTu { get; set; }
        public long BenhNhanId { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string TenGioiTinh { get; set; }
        public int Tuoi { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public bool CoBaoHiem { get; set; }

        //public long? YeuCauDichVuKyThuatId { get; set; }
        //public string DiaChi { get; set; }
        //public string LyDoKham { get; set; }
        //public string TenNhomMau { get; set; }
        //public Enums.EnumNhomMau? NhomMau { get; set; }
        //public int? Mach { get; set; }
        //public  int? NhipTho { get; set; }
        //public double? CanNang { get; set; }
        //public int? HuyetAp { get; set; }
        //public string HuyetApDisplay { get; set; }
        //public double? NhietDo { get; set; }
        //public double? ChieuCao { get; set; }
        //public double? BMI { get; set; }
        //public string TinhTrang { get; set; }
        //public bool? ProgessChiSoSinhTon { get; set; }

        //public long PhongBenhVienId { get; set; }
        //public Enums.EnumTrangThaiHangDoi TrangThai { get; set; }
        //public Enums.EnumTrangThaiYeuCauTiepNhan TrangThaiYeuCauTiepNhan { get; set; }
        //public DateTime ThoiDiemTiepNhan { get; set; }
        //public Enums.EnumLoaiHangDoi LoaiHangDoi { get; set; }
        //public ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats { get; set; }
    }
    public class LichSuPhauThuatThuThuatGridVo : GridItem
    {
        public long? YeuCauTiepNhanId { get; set; }
        public long? BenhNhanId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string DiaChi { get; set; }
        public string TenDichVu { get; set; }
        public string TrieuChungTiepNhan { get; set; }
        public string DoiTuong { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ThoiDiemTiepNhanTu { get; set; }
        public string ThoiDiemTiepNhanDen { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public string ThoiDiemTiepNhanDisplay { get; set; }
        public string TrangThaiPTTTSearch { get; set; }
        public string SearchString { get; set; }
        public EnumTuVongPTTTTheoNgay? TuVongTrongPTTT { get; set; }
        public EnumThoiGianTuVongPTTTTheoNgay? KhoangThoiGianTuVong { get; set; }
        public int? LanThucThien { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien?.ApplyFormatDateTime();

        public DateTime? ThoiDiemHoanThanh { get; set; }
        public string ThoiDiemHoanThanhDisplay => ThoiDiemHoanThanh?.ApplyFormatDateTime();
        public List<long> TuongTrinhLaiYeuCauKyThuatIds { get; set; }
        public string NoiChuyenGiao { get; set; }
        public bool DuocTuongTrinhLai { get; set; }
        public long? PhongBenhVienId { get; set; }

        //BVHD-3860
        public DanhSachKhamPTHoanThanhThucHienVo ThongTinThucHien { get; set; }
        public int SoDichVuKhongTuongTrinh { get; set; }
        public bool? LaKhongThucHien { get; set; }
    }

    #region BVHD-3860

    public class DanhSachKhamPTHoanThanhThucHienVo
    {
        public bool? ThucHien { get; set; }
        public bool? KhongThucHien { get; set; }
    }


    #endregion

    public class ThongTinBenhNhanPTTTVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBN { get; set; }
        public long? BenhNhanId { get; set; }
        public string HoTen { get; set; }
        public string TenGioiTinh { get; set; }
        public int? Tuoi { get; set; }
        public string SoDienThoai { get; set; }
        public string DanToc { get; set; }
        public string DiaChi { get; set; }
        public string NgheNghiep { get; set; }
        public string TuyenKham { get; set; }
        public int? BHYTMucHuong { get; set; }
        public string TenLyDoTiepNhan { get; set; }
        public string TenLyDoKhamBenh { get; set; }
        public string ThoiDiemTiepNhanDisplay { get; set; }
        public int? NamSinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NgaySinh { get; set; }
        public string TuoiThoiDiemHienTai { get; set; }
        public EnumTuVongPTTTTheoNgay? TrangThai { get; set; }
        public long? TheoDoiSauPhauThuatThuThuatId { get; set; }
        public List<ThongTinKhamKhacChiTietVo> ThongTinKhamKhacChiTiet { get; set; }
        public bool? CoTuongTrinhPTTT { get; set; }
        public int? LanThucThien { get; set; }
        public bool? TrangThaiTuVongTiepNhan { get; set; }
        public bool? TrangThaiTuVongTuongTrinh { get; set; }
        public string SoBHYT { get; set; }
        public bool? CoBHYT { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public string BHYTThoiGianHieuLucDisplay => BHYTNgayHieuLuc?.ApplyFormatDate() + (BHYTNgayHieuLuc != null && BHYTNgayHetHan != null ? " - " : "") + BHYTNgayHetHan?.ApplyFormatDate();
        public bool IsBHYTHetHan => CoBHYT == true && BHYTNgayHieuLuc != null && BHYTNgayHetHan != null && (DateTime.Now.Date < BHYTNgayHieuLuc.Value.Date || DateTime.Now.Date > BHYTNgayHetHan.Value.Date);
        public bool CoPhauThuat { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }

        //BVHD-3941
        public bool? CoBaoHiemTuNhan { get; set; }
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }

    public class LichSuPhauThuatThuThuatExportExcel
    {
        [Width(30)]
        public string MaYeuCauTiepNhan { get; set; }
        [Width(20)]
        public string MaBN { get; set; }
        [Width(40)]
        public string HoTen { get; set; }
        [Width(20)]
        public int? NamSinh { get; set; }
        //[Width(60)]
        //public string DiaChi { get; set; }
        [Width(30)]
        public string ThoiDiemTiepNhanDisplay { get; set; }
        [Width(40)]
        public string TrieuChungTiepNhan { get; set; }
        [Width(40)]
        public string DoiTuong { get; set; }
        //[Width(30)]
        //public string TenDichVu { get; set; }
        [Width(30)]
        public string TrangThaiPTTTSearch { get; set; }
        [Width(30)]
        public string ThoiDiemThucHienDisplay { get; set; }
        [Width(30)]
        public string ThoiDiemHoanThanhDisplay { get; set; }
        [Width(30)]
        public string NoiChuyenGiao { get; set; }
    }

    #region CLS
    public class LichSuCLSPTTTVo : GridItem
    {
        public LichSuCLSPTTTVo()
        {
            FileKetQuaCanLamSangs = new List<FileKetQuaCanLamSangVo>();
        }
        public int? STT { get; set; }
        public string Nhom { get; set; }
        public int NhomId { get; set; }
        public int TrangThaiDichVuId { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public string TenLoaiGia { get; set; }
        public decimal? DonGia { get; set; }
        public int SoLan { get; set; }
        public string SoLanDichVuThucHien { get; set; }
        public decimal? ThanhTien { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public string DuocHuongBaoHiem { get; set; }
        public bool? DHBH { get; set; }
        public long? NoiThucHienId { get; set; }
        public string NoiThucHien { get; set; }
        public string TrangThai { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string NhanVienThucHien { get; set; }
        public List<FileKetQuaCanLamSangVo> FileKetQuaCanLamSangs { get; set; }
        public bool CoFileKetQuaCanLamSangWordExcel { get; set; }
        public bool CoFileKetQuaCanLamSang { get; set; }
        public string ThoiDiemThucHienDisplay { get; set; }
        public string TenNhanVienChiDinh { get; set; }
        public string ThoiDiemChiDinhDisplay { get; set; }

        public long? LoaiDichVuKyThuat { get; set; }
        public long? LoaiYeuCauDichVuId { get; set; }

    }
    public class FileKetQuaCanLamSangVo : GridItem
    {
        public string TenGuid { get; set; }
        public string Url { get; set; }
        public string DuongDan { get; set; }
        public string Ten { get; set; }
        public int LoaiFile { get; set; }
        public string MoTa { get; set; }
        public bool? IsDownload { get; set; }
    }

    #endregion

    #region TuongTrinh
    public class LichSuDichVuKyThuatDaTuongTrinhVo
    {
        public long? YeuCauTiepNhanId { get; set; }
        public int? LanThucThien { get; set; }
        public int? PhongBenhVienId { get; set; }
    }

    public class LichSuDichVuKyThuatDaTuongTrinhPTTT
    {
        public int DichVuKyThuatDaTuongTrinh { get; set; }
        public int TongDichVuKyThuat { get; set; }
    }
    #endregion

    #region Ekip
    public class LichSuEkipPTTTVo : GridItem
    {
        public long? KhoaPhongId { get; set; }
        public string TenKhoaPhong { get; set; }
        public string ChanDoanVaoKhoa { get; set; }
        public string MoTaChanDoan { get; set; }
        public string ChanDoanTruocPT { get; set; }
        public string ThoiGianTruocPT { get; set; }
        public string TenDichVu { get; set; }
        public string MoTaTruocPT { get; set; }
        public string ChanDoanSauPT { get; set; }
        public string MoTaSauPT { get; set; }
        public string PhuongPhapPTTT { get; set; }
        public string PhuongPhapVoCam { get; set; }
        public string PhuongPhapPTVien { get; set; }
        public string TinhHinhPTTT { get; set; }
        public string TaiBienPTTT { get; set; }
        public string TuVongTrongPTTT { get; set; }
        public string TrinhTuPT { get; set; }
        public string LuocDoPT { get; set; }
        public string LoaiPhauThuatThuThuat { get; set; }
        public string ThoiDiemPhauThuat { get; set; }
        public string ThoiGianBatDauGayMe { get; set; }
        public string ThoiDiemKetThucPhauThuat { get; set; }
        public string PTTTVienChinh { get; set; }
        public bool? DichVuKhongThucHien { get; set; }
        public List<LichSuImgLuocDoPTVo> ImgLuocDoPT { get; set; }

        //BVHD-3877
        public string GhiChuCaPTTT { get; set; }
    }
    public class LichSuImgLuocDoPTVo
    {
        public string LuocDo { get; set; }
        public string MoTa { get; set; }
    }

    public class LichSuNguoiThucHienEkipPTTTVo : GridItem
    {
        public string ChucDanh { get; set; }
        public string HoTen { get; set; }
        public string VaiTro { get; set; }
    }
    public class DichVuLookupItem
    {
        public long YeuCauDichVuKyThuatId { get; set; }
        public string DisplayName { get; set; }
    }
    #endregion

    #region KetLuanPTTT
    public class LichSuKetLuanPTTTVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long TheoDoiSauPhauThuatThuThuatId { get; set; }
        public long TinhTrangTheoDoiId { get; set; }
        public string BacSiPhuTrachTheoDoi { get; set; }
        public EnumTrangThaiTheoDoiSauPhauThuatThuThuat? TinhTrang { get; set; }
        public string TinhTrangDisplay { get; set; }
        public string DieuDuongPhuTrachTheoDoi { get; set; }
        public string ThoiDiemTheoDoi { get; set; }
        public string GhiChuTheoDoi { get; set; }
        public long? BacSiPhuTrachTheoDoiId { get; set; }
        public long? DieuDuongPhuTrachTheoDoiId { get; set; }

    }
    #endregion

    #region TheoDoi
    public class LichSuHoiTinhChiSoSinhTonPTTTVo : GridItem
    {
        public int? NhipTim { get; set; }
        public int? NhipTho { get; set; }
        public string HuyetAp { get; set; }
        public double? ThanNhiet { get; set; }
        public double? ChieuCao { get; set; }
        public double? CanNang { get; set; }
        public double? BMI { get; set; }
        public string NhanVienThucHien { get; set; }
        public string NgayThucHien { get; set; }
        public double? Glassgow { get; set; }
        public double? SpO2 { get; set; }
    }

    public class LichSuTheoDoiKhamCoQuanKhacPTTTVo : GridItem
    {
        public string BoPhan { get; set; }
        public string MoTa { get; set; }
    }
    public class ThongTinKhamKhacChiTietVo : GridItem
    {
        public string ThongTinKhamTheoDoiTemplate { get; set; }
        public string ThongTinKhamTheoDoiData { get; set; }
        public string KhamToanThan { get; set; }
    }
    #endregion

    #region DichVuPTTTLookup
    public class DichVuPTTTsLookupItemVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public long? BacSiChinhId { get; set; }
        public string BacSiChinh { get; set; }
        public bool TrangThai { get; set; }
        public string TrangThaiDisplay => TrangThai ?
            "Đã tường trình" : "Đang tường trình";
        public string LoaiPhauThuatThuThuat { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public string LoaiPTTT { get; set; }
        //public string LoaiPTTT => !string.IsNullOrEmpty(LoaiPhauThuatThuThuat) && LoaiPhauThuatThuThuat.Substring(0, 1).ToLower().Contains("p") ? "Phẫu thuật" : "Thủ thuật";
    }
    #endregion

    #region DichVuPTTTKhongThucHien
    public class LichSuDichVuPTTTKhongThucHienGridVo : GridItem
    {
        public long? BacSiHuyId { get; set; }
        public string TenDichVu { get; set; }
        public string HoTenBacSiHuy { get; set; }
        public string LyDo { get; set; }
        public int? LanThucThien { get; set; }
    }
    #endregion

    #region Danh sách hoàn thành phẩu thuật thủ thuật
    public class DanhSachHoanThanhPTTTVo : GridItem
    {
        public long? PhongBenhVienId { get; set; }
        public long? NoiTiepNhanId { get; set; }

        public long YeuCauTiepNhanId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBN { get; set; }
        public long? BenhNhanId { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string DiaChi { get; set; }
        public string DoiTuong { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }
        public string ThoiDiemHoanThanhStr => ThoiDiemHoanThanh?.ApplyFormatDateTimeSACH();
        public bool TuVongTrongPTTT { get; set; }
        public string TrangThaiPTTTSearch { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ThoiDiemTiepNhanTu { get; set; }
        public string ThoiDiemTiepNhanDen { get; set; }
        //sreach
        public int? SoLan { get; set; }
        public string SearchString { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien?.ApplyFormatDateTime();
        public string NoiChuyenGiao { get; set; }
        public bool? DuocTuongTrinhLai { get; set; }
        public List<long> TuongTrinhLaiYeuCauKyThuatIds { get; set; }

        //BVHD-3860
        public int SoDichVuKhongTuongTrinh { get; set; }
        public bool? LaKhongThucHien { get; set; }
    }
    public class TuongTrinhLai
    {
        public List<long> TuongTrinhLaiTheoYeuCauDichVuKyThuatId { get; set; }
        public long PhongBenhVienId { get; set; }
    }


    #endregion
}
