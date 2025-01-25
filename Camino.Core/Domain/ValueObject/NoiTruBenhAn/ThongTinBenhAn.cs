using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.NoiTruBenhAn
{
    public class ThongTinBenhAn
    {
        public ThongTinBenhAn()
        {
            ChiSoSinhTons = new List<ChiSoSinhTon>();
            DacDiemTreSoSinhSauSinhs = new List<DacDiemTreSoSinhSauSinh>();
        }

        public long YeuCauTiepNhanId { get; set; }
        public string LyDoVaoVien { get; set; }
        public string ThuMayVaoVien { get; set; }
        public string QuaTrinhHoiBenh { get; set; }
        public string TienSuBenhBanThan { get; set; }
        public List<TienSuBenhLienQuan> TienSuBenhLienQuans { get; set; }
        public string TienSuBenhGiaDinh { get; set; }

        public string KhamBenhToanThan { get; set; }
        public List<ChiSoSinhTon> ChiSoSinhTons { get; set; }

        public string TuanHoan { get; set; }
        public string HoHap { get; set; }
        public string TieuHoa { get; set; }
        public string ThanTietNieu { get; set; }
        public string ThanKinh { get; set; }
        public string CoXuongKhop { get; set; }
        public string SanPhuKhoa { get; set; }
        public string DaLieu { get; set; }
        public string TaiMuiHong { get; set; }
        public string RangHamMat { get; set; }
        public string Mat { get; set; }
        public string NoiTiet { get; set; }
        public string CacXetNghiemCanLam { get; set; }
        public string TomTatBenhAn { get; set; }

        public long? ICDChinh { get; set; }
        public string TenICDChinh { get; set; }
        public string ChuanDoan { get; set; }

        public List<ThongTinChuanDoanKemTheo> ChuanDoanKemTheos { get; set; }
        public List<ThongTinChuanDoanKemTheo> ChuanDoanPhanBiets { get; set; }

        public string TienLuong { get; set; }
        public string HuongDanDieuTri { get; set; }

        public string KyHieuDiUng => "Dị ứng";
        public decimal? ThoiGianDiUng { get; set; }
        public string KyHieuMaTuy => "Ma túy";
        public decimal? ThoiGianMaTuy { get; set; }
        public string KyHieuRuouBia => "Rượu bia";
        public decimal? ThoiGianRuouBia { get; set; }

        public string KyHieuThuocLa => "Thuốc lá";
        public decimal? ThoiGianThuocLa { get; set; }
        public string KyHieuThuocLao => "Thuốc lào";
        public decimal? ThoiGianThuocLao { get; set; }
        public string KyHieuKhac => "Khác";
        public decimal? ThoiGianKhac { get; set; }

        public List<BoPhanTonThuong> BoPhanTonThuongs { get; set; }

        public int? BatDauThayKinhNam { get; set; }
        public int? TuoiCoKinh { get; set; }
        public string TinhChatKinhNghiem { get; set; }
        public int? ChuKy { get; set; }
        public int? SoNgayThayKinh { get; set; }
        public string LuongKinh { get; set; }
        public int? NamSinh { get; set; }

        public string GhiChuPara { get; set; }
        public int? Para { get; set; }
        public string KinhCuoiNgayText { get; set; }
        public DateTime? KinhCuoiNgay { get; set; }

        public bool? DauBung { get; set; }
        public bool? ThoiGianTruoc { get; set; }
        public bool? ThoiGianTrong { get; set; }
        public bool? ThoiGianSau { get; set; }

        public int? LayChongNam { get; set; }
        public int? TuoiLayChong { get; set; }

        public int? NamHetKinh { get; set; }
        public int? TuoiHetKinh { get; set; }

        public string BenhPhuKhoaDieuTri { get; set; }


        public Enums.TienThaiPara? TienThaiPara { get; set; }

        public string DaNienMac { get; set; }
        public string Hach { get; set; }
        public string Vu { get; set; }

        public string ThanTieuNieu { get; set; }
        public string Khac { get; set; }

        public string CacDauHieuSinhDucThuPhat { get; set; }
        public string MoiLon { get; set; }
        public string MoiBe { get; set; }
        public string AmVat { get; set; }
        public string AmHo { get; set; }
        public string MangTrinh { get; set; }
        public string TangSinhMon { get; set; }

        public string AmDao { get; set; }
        public string CoTuCung { get; set; }
        public string ThanTuCung { get; set; }
        public string PhanPhu { get; set; }
        public string CacTuiCung { get; set; }

        public int? SoToXQuang { get; set; }
        public int? SoToCTScanner { get; set; }
        public int? SoToSieuAm { get; set; }
        public int? SoToXetNghiem { get; set; }

        public int? SoToKhac { get; set; }
        public RangeDate TuNgayDenNgay { get; set; }

        public int? TuoiThai { get; set; }
        public string KhamLaiThai { get; set; }
        public bool? TiemChungUongVan { get; set; }
        public int? DuocTiem { get; set; }
        public DateTime? BatDauChuyenDa { get; set; }
        public string DauHieuBanDau { get; set; }
        public string ChuyenBien { get; set; }
        public string NhungBenhPhuKhoaDaDieuTri { get; set; }
        public string ToanTrang { get; set; }
        public bool Phu { get; set; }

        public string CacBoPhanKhac { get; set; }
        public bool BungCoSoCu { get; set; }
        public double? ChieuCaoTuCung { get; set; }
        public double? VongBung { get; set; }
        public int? TimThai { get; set; }
        public string HinhDangTuCung { get; set; }
        public string TuThe { get; set; }
        public string ConCoTC { get; set; }

        public double? ChiSoBiShop { get; set; }

        public DateTime? NgayVoNuocOi { get; set; }
        public string MauSacNuocOi { get; set; }
        public string Ngoi { get; set; }
        public string The { get; set; }

        public Enums.DoLot? DoLotId { get; set; }
        public string TenDoLot { get; set; }
        public Enums.TinhTrangVoOi? TinhTrangVoOiId { get; set; }
        public string TenTinhTrangVoOi { get; set; }
        public Enums.VoOi? VoOiId { get; set; }
        public string TenVoOi { get; set; }
        public string NuocOiNhieuHayIt { get; set; }
        public string KieuThe { get; set; }
        public string DuongKinhNhoHaVe { get; set; }
        public string CacXetNghiemCLS { get; set; }

        public DateTime? VaoBuongDeLuc { get; set; }
        public int? NhanVienTheoDoiId { get; set; }
        public string TenNhanVienTheoDoi { get; set; }
        public int? ChucDanhId { get; set; }
        public string TenChucDanh { get; set; }

        public string TinhTrangSauKhiDe { get; set; }
        public string XuLyKetQua { get; set; }
        public bool? Boc { get; set; }
        public bool? So { get; set; }
        public DateTime? RauSoLuc { get; set; }
        public string CachSoRau { get; set; }
        public string MatMang { get; set; }
        public string MatMui { get; set; }
        public string BanhRau { get; set; }
        public double? CanNang { get; set; }
        public bool? RauCuonCo { get; set; }
        public string CuonRauDai { get; set; }
        public bool? CoChayMauSauSo { get; set; }
        public int? LuongMauMat { get; set; }
        public bool? KiemSoatTuCung { get; set; }
        public string XuLyKetQuaSoRau { get; set; }

        public string DaMienMac { get; set; }
        public Enums.PhuongPhapDe? PhuongPhapDeId { get; set; }
        public string TenPhuongPhapDe { get; set; }
        public string LyDoCanThiep { get; set; }

        public Enums.TangSinhMon? TangSinhMonId { get; set; }
        public string TenTangSinhMon { get; set; }
        public string PhuongPhapKhauVaLoaiChi { get; set; }
        public int? SoMuiKhau { get; set; }
        public Enums.CoTuCung? CoTuCungId { get; set; }
        public string TenCoTuCung { get; set; }
        public string ChanDoanTruocPhauThuat { get; set; }
        public string ChanDoanSauPhauThuat { get; set; }

        public List<LanPhauThuat> LanPhauThuats { get; set; }
        public bool? TrieuChung { get; set; }
        public bool? TaiBien { get; set; }
        public bool? BienChung { get; set; }
        public bool? DoPhauThuat { get; set; }
        public bool? DoGayMe { get; set; }
        public bool? DoViKhuan { get; set; }
        public string HuongDieuTriCacCheDoTiepTheo { get; set; }

        public List<TienSuSanKhoa> TienSuSanKhoas { get; set; }

        public DateTime? NgayVoOi { get; set; }
        public string MauSac { get; set; }
        public bool? CachDe { get; set; }
        public DateTime? DeLuc { get; set; }
        public Enums.TinhTrangSoSinh? TinhTrangSoSinh { get; set; }
        public int? NhanVienId { get; set; }
        public string TenNhanVien { get; set; }
        public string Apgar1p { get; set; }
        public string Apgar5p { get; set; }
        public string Apgar10p { get; set; }
        public string TinhTrangDinhDuongSauSinh { get; set; }

        public bool? HutDich { get; set; }
        public bool? XoaBopTim { get; set; }
        public bool? ThoO2 { get; set; }
        public bool? DatNoiKhiQuan { get; set; }
        public bool? BopBongO2 { get; set; }

        public int? NhanVienChuyenSoSinhId { get; set; }
        public string TenNhanVienChuyenSoSinh { get; set; }

        public int? ChucDanhNhanVienId { get; set; }
        public string TenNhanVienChucDan { get; set; }

        public bool? DiTatBamSinh { get; set; }
        public string ChuThichDiTatBamSinh { get; set; }
        public bool? CoHauMon { get; set; }

        public string TinhHinhSoSinhKhiVaoKhoa { get; set; }
        public string TinhHinhToanThan { get; set; }

        public Enums.MauSacDa? TrangThaiMauSacDa { get; set; }
        public string ChuThichMauSacDa { get; set; }

        public int? HoHapNhiTho { get; set; }
        public double? VongDauSoSinh { get; set; }

        public string HoHapNgheTho { get; set; }
        public int? HoHapChiSoSilverman { get; set; }

        public int? TimMachNhipTim { get; set; }

        public string Bung { get; set; }
        public string CacCoQuanSinhDucNgoai { get; set; }
        public string XuongKhop { get; set; }

        public string ThanKinhPhanXa { get; set; }
        public string ThanKinhTruongLucCo { get; set; }
        public string ChiDinhTheoDoi { get; set; }



        public int? NuoiDuong { get; set; }
        public int? CaiSuaThangThu { get; set; }
        public Enums.ChamSoc? ChamSoc { get; set; }
        public bool? Lao { get; set; }
        public bool? BaiLiet { get; set; }
        public bool? Soi { get; set; }
        public bool? HoGa { get; set; }
        public bool? UonVang { get; set; }
        public bool? BachHau { get; set; }
        public string NoiDungTiemChungKhac { get; set; }

        public double? ChieuCao { get; set; }
        public double? VongNguc { get; set; }
        public double? VongDau { get; set; }
        public string ThongTinKhamBenhToanThan { get; set; }

        public double? CanNangLucSinh { get; set; }
        public string ThongTinDiTatBamSinh { get; set; }
        public string PhatTrienVeTinhThan { get; set; }
        public string PhatTrienVeVanDong { get; set; }
        public string CacBenhLyKhac { get; set; }
        public int? ConThuMay { get; set; }
        public int? TienThaiPara1 { get; set; }
        public int? TienThaiPara2 { get; set; }
        public int? TienThaiPara3 { get; set; }
        public int? TienThaiPara4 { get; set; }
        public Enums.TinhTrangKhiSinh? TinhTrangKhiSinh { get; set; }
        public Enums.DeThuong? DeThuong { get; set; }
        public bool? Forceps { get; set; }
        public bool? GiacHut { get; set; }
        public bool? DeChiHuy { get; set; }

        public List<DacDiemTreSoSinhSauSinh> DacDiemTreSoSinhSauSinhs { get; set; }
        public string HuongXuLyLoiDanBs { get; set; }
    }



    public class TienSuBenhLienQuan : GridItem
    {
        public string KyHieu { get; set; }
        public int? ThoiGian { get; set; }
    }

    public class ChiSoSinhTon : GridItem
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

        public double? HuyetApTamThu { get; set; }
        public double? HuyetApTamTruong { get; set; }

    }

    public class TienSuSanKhoa
    {
        public int? SoLanCoThai { get; set; }
        public int? Nam { get; set; }
        public bool? DeDuThang { get; set; }
        public bool? DeThieuThang { get; set; }
        public bool? Say { get; set; }
        public bool? Hut { get; set; }
        public bool? Nao { get; set; }
        public bool? CoCVat { get; set; }
        public bool? ChuaNgoaiTC { get; set; }
        public bool? ChuaTrung { get; set; }
        public bool? ThaiChetLuu { get; set; }
        public bool? ConHienSong { get; set; }

        public double? CanNang { get; set; }
        public string PhuongPhapDe { get; set; }
        public bool? TaiBien { get; set; }
    }

    public class ThongTinChuanDoanKemTheo : GridItem
    {
        public long ICD { get; set; }
        public string TenICD { get; set; }
        public string ChuanDoan { get; set; }
    }

    public class DacDiemTreSoSinhSauSinh : GridItem
    {
        public long Diem { get; set; }
        public string SuGianNoLongNguc { get; set; }
        public string CoKeoCoLienSuon { get; set; }
        public string CoKeoMuiUc { get; set; }
        public string DapCanhMui { get; set; }
        public string Reni { get; set; }
    }

    public class CoQuanKhac
    {
        public string TuanHoan { get; set; }
        public string HoHap { get; set; }
        public string TieuHoa { get; set; }
        public string ThanTietNieuSinhDuc { get; set; }
        public string ThanKinh { get; set; }
        public string CoXuongKhop { get; set; }
        public string RangHamMat { get; set; }
        public string NoiTietDinhDuong { get; set; }
        public string SanPhuKhoa { get; set; }
        public string DaLieu { get; set; }
        public string TaiMuiHong { get; set; }
        public string Mat { get; set; }
        public string ThanTietNieu { get; set; }
        public string Khac { get; set; }
    }

    public class BoPhanTonThuong
    {
        public int Id { get; set; }
        public string MoTa { get; set; }
        public string HinhAnh { get; set; }
    }
    public class LanPhauThuat
    {
        public DateTime? NgayGio { get; set; }
        public string NgayGioDisplay { get; set; }
        public string PhuongPhapPhauThuat { get; set; }
        public string PTTT { get; set; }
        public string VoCam { get; set; }

        public int? PhauThuatVienId { get; set; }
        public string TenPhauThuatVien { get; set; }

        public int? BacSiGayMeId { get; set; }
        public string TenBacSiGayMe { get; set; }
    }


}