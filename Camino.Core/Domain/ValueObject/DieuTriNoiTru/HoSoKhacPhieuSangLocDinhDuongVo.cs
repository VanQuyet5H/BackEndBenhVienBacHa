using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class HoSoKhacPhieuSangLocDinhDuongGridVo : GridItem
    {
        public float ChieuCaoTu { get; set; }
        public float? ChieuCaoDen { get; set; }
        public string ChieuCaoDisplay => ChieuCaoDen != null ? $"{ChieuCaoTu} - {ChieuCaoDen}m" : $"> {ChieuCaoTu}m";
        public int CanNangTu { get; set; }
        public int CanNangDen { get; set; }
        public string CanNangDisplay => $"{CanNangTu} - {CanNangDen}kg";
        public int NangLuongTu { get; set; }
        public int NangLuongDen { get; set; }
        public string NangLuongDisplay => $"{NangLuongTu} - {NangLuongDen}kcal";
        public int DamTu { get; set; }
        public int DamDen { get; set; }
        public string DamDisplay => $"{DamTu} - {DamDen}g đạm";
    }

    public class HoSoKhacPhieuSangLocDinhDuongVo
    {
        public HoSoKhacPhieuSangLocDinhDuongVo()
        {
            lstNhuCauDinhDuong = new List<HoSoKhacPhieuSangLocDinhDuongNhuCauDinhDuongVo>();
        }

        public bool DungChoPhuNuMangThai { get; set; }
        public DateTime? NgayDanhGia { get; set; }
        public string ChanDoan { get; set; }

        public string NguoiThucHienName { get; set; }
        public int? NguoiThucHienId { get; set; }

        public int? CanNang { get; set; }
        public int? ChieuCao { get; set; }
        public float? BMI { get; set; }
        public string DaiThamChieuBMI { get; set; }
        public string MucDoBMI { get; set; }
        public GiamCan? GiamCan { get; set; }
        public string GiamCanDisplay { get; set; }
        public int? DiemGiamCan { get; set; }
        public SoKgGiam? SoKgGiam { get; set; }
        public string SoKgGiamDisplay { get; set; }
        public int? DiemSoKgGiam { get; set; }
        public AnUongKem? AnUongKem { get; set; }
        public string AnUongKemDisplay { get; set; }
        public int? DiemAnUongKem { get; set; }
        public bool? BoSungDinhDuongDuongUong { get; set; }
        public string LoiKhuyenDinhDuongKhac { get; set; }
        public TinhTrangBenhLyNang? TinhTrangBenhLyNang { get; set; }
        public string TinhTrangBenhLyNangDisplay { get; set; }
        public int? TongDiemMST { get; set; }
        public string ChanDoanMucDoSuyDinhDuong { get; set; }

        public int? NhuCauKhacNangLuong { get; set; }
        public int? NhuCauKhacDuong { get; set; }
        public int? NhuCauKhacDam { get; set; }
        public int? NhuCauKhacBeo { get; set; }
        public KeHoachDinhDuong? KeHoachDinhDuong { get; set; }
        public string KeHoachDinhDuongDisplay { get; set; }

        public List<HoSoKhacPhieuSangLocDinhDuongNhuCauDinhDuongVo> lstNhuCauDinhDuong { get; set; }

        public int? TuoiThai { get; set; }
        public bool? TheoKinhCuoiCung { get; set; }
        public bool? SieuAmBaThangDauThaiKy { get; set; }
        public int? CanNangTruocMangThai { get; set; }
        public int? ChieuCaoTruocMangThai { get; set; }
        public float? BMITruocMangThai { get; set; }
        public int? CanNangHienTai { get; set; }
        public string KhoangBMITruocMangThai { get; set; }
        public int? DiemBMITruocMangThai { get; set; }
        public TocDoTangCan? TocDoTangCan { get; set; }
        public string TocDoTangCanDisplay { get; set; }
        public int? DiemTocDoTangCan { get; set; }
        public BenhKemTheo? BenhKemTheo { get; set; }
        public string BenhKemTheoDisplay { get; set; }
        public int? DiemBenhKemTheo { get; set; }
        public int? TongDiemTruocMangThai { get; set; }
        public string KetLuan { get; set; }



       public SutCan1ThangQua? SutCan1ThangQua { get; set; }
       public AnKemLonHon5Ngay? AnKemLonHon5 { get; set; }
       public long? CheDoAnUong { get; set; }
       public DuongNuoiDuong? DuongNuoiDuong { get; set; }
       public HoiChanDinhDuong? HoiChanDinhDuong { get; set; }
       public TaiDanhGia? TaiDanhGia { get; set; }
        public string NgayDanhGiaString { get; set; }
        public long? NoiTruHoSoKhacId { get; set; }

        public string BacSiDieuTriName { get; set; }
        public long? BacSiDieuTriId { get; set; }
    }

    public class HoSoKhacPhieuSangLocDinhDuongNhuCauDinhDuongVo : GridItem
    {
        public bool isCheck { get; set; }
    }

    public class HoSoKhacPhieuInSangLocDinhDuong
    {
        public string LogoUrl { get; set; }
        public string KhoaPhong { get; set; }
        public string HoTen { get; set; }
        public string GioiTinhNam { get; set; }
        public string GioiTinhNu { get; set; }
        public string NgaySinh { get; set; }
        public string Tuoi { get; set; }
        public string NgheNghiep { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string NgayVaoVien { get; set; }
        public float? BMI { get; set; }
        public string ChanDoan { get; set; }
        public string GiamCanKhong { get; set; }
        public string GiamCanKhongRo { get; set; }
        public string SoKgGiamMotDenNamKg { get; set; }
        public string SoKgGiamSauDenMuoiKg { get; set; }
        public string SoKgGiamMuoiMotDenMuoiLamKg { get; set; }
        public string SoKgGiamMuoiLamKgTroLen { get; set; }
        public string SoKgGiamKhongRo { get; set; }
        public string AnUongKemKhong { get; set; }
        public string AnUongKemCo { get; set; }
        public string BoSungDinhDuongDuongUongCo { get; set; }
        public string BoSungDinhDuongDuongUongKhong { get; set; }
        public string LoiKhuyenDinhDuongKhac { get; set; }
        public int? TongDiem { get; set; }

        public string KetLuanKhongCo3CS { get; set; }
        public string KetLuanCo1Trong3CS { get; set; }
        public string NhuCauDinhDuong145 { get; set; }
        public string NhuCauDinhDuong151 { get; set; }
        public string NhuCauDinhDuong156 { get; set; }
        public string NhuCauDinhDuong161 { get; set; }
        public string NhuCauDinhDuong166 { get; set; }
        public string NhuCauDinhDuong171 { get; set; }
        public string NhuCauDinhDuong175 { get; set; }
        public int? NhuCauKhacNangLuong { get; set; }
        public int? NhuCauKhacDuong { get; set; }
        public int? NhuCauKhacDam { get; set; }
        public int? NhuCauKhacBeo { get; set; }
        public string KeHoachDinhDuongDuongMieng { get; set; }
        public string KeHoachDinhDuongQuaOngThong { get; set; }
        public string KeHoachDinhDuongQuaTinhMach { get; set; }

        public int Ngay { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string NguoiThucHien { get; set; }
        public string MaTN { get; set; }
        public string KhoaDangIn { get; set; }
        public string BarCodeImgBase64 { get; set; }
    }

    public class HoSoKhacPhieuInSangLocDinhDuongChoPhuNuMangThai
    {
        public string ValueHoTenClass => !string.IsNullOrWhiteSpace(HoTen) ? "value_khong_gach_chan" : "value";
        public string ValueNamSinhClass => !string.IsNullOrWhiteSpace(NamSinh.ToString()) ? "value_khong_gach_chan" : "value";
        public string ValueDiaChiClass => !string.IsNullOrWhiteSpace(DiaChi) ? "value_khong_gach_chan" : "value";
        public string ValueTuoiThaiClass => !string.IsNullOrWhiteSpace(TuoiThai.ToString()) ? "value_khong_gach_chan" : "value";
        public string ValueSoBuongClass => !string.IsNullOrWhiteSpace(SoBuong) ? "value_khong_gach_chan" : "value";
        public string ValueSoBenhAnClass => !string.IsNullOrWhiteSpace(SoBenhAn) ? "value_khong_gach_chan" : "value";
        public string ValueSoGiuongClass => !string.IsNullOrWhiteSpace(SoGiuong) ? "value_khong_gach_chan" : "value";
        public string ValueChuanDoanClass => !string.IsNullOrWhiteSpace(ChanDoan) ? "value_khong_gach_chan" : "value";
        public string ValueCanNangClass => !string.IsNullOrWhiteSpace(CanNang.ToString()) ? "value_khong_gach_chan" : "value";
        public string ValueChieuCaoClass => !string.IsNullOrWhiteSpace(ChieuCao.ToString()) ? "value_khong_gach_chan" : "value";
        public string ValueBMIClass => !string.IsNullOrWhiteSpace(BMI.ToString()) ? "value_khong_gach_chan" : "value";
        public string ValueCanNangHienTaiClass => !string.IsNullOrWhiteSpace(CanNangHienTai.ToString()) ? "value_khong_gach_chan" : "value";

        public string LogoUrl { get; set; }
        public string HoTen { get; set; }
        public string KhoaPhong => "Khoa phụ sản";
        public string NamSinh { get; set; }
        public string DiaChi { get; set; }
        public int? TuoiThai { get; set; }
        public string TheoKinhCuoiCung { get; set; }
        public string BaThangDauThaiKy { get; set; }
        public string SoBuong { get; set; }
        public string SoGiuong { get; set; }
        public string SoBenhAn { get; set; }
        public string ChanDoan { get; set; }
        public int? CanNang { get; set; }
        public float? ChieuCao { get; set; }
        public float? BMI { get; set; }
        public string BMIDisplay { get; set; }
        public int? CanNangHienTai { get; set; }
        public string BMITruocMangThaiBT185N249 { get; set; }
        public string BMITruocMangThaiGE25 { get; set; }
        public string BMITruocMangThaiLT185 { get; set; }
        public string TocDoTangCanTheoKhuyenNghi { get; set; }
        public string TocDoTangCanTrenDuoiMucKhuyenNghi { get; set; }
        public string BenhKemTheoKhong { get; set; }
        public string BenhKemTheoTangHuyetAp { get; set; }
        public string KetLuanBinhThuong { get; set; }
        public string KetLuanCoNguyCoDinhDuong { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string NguoiThucHien { get; set; }
        public string MaTN { get; set; }
        public string KhoaDangIn { get; set; }
        public string BarCodeImgBase64 { get; set; }
    }
    public class DataInPhieuInSangLocDinhDuongVaSerivcesVo
    {
        public string MaSoTiepNhan { get; set; }

        public string Khoa { get; set; }

        public string MaBn { get; set; }

        public string NhomMau { get; set; }

        public string HoTenNgBenh { get; set; }

        public int? NamSinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NgaySinh { get; set; }

        public int TuoiNgBenh => NamSinh != null ? DateTime.Now.Year - NamSinh.GetValueOrDefault() : 0;

        public string Cmnd { get; set; }

        public string GTNgBenh { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string Buong { get; set; }

        public string Giuong { get; set; }

        public string ChanDoan { get; set; }

        public string DuKienPPDieuTri { get; set; }

        public string TienLuong { get; set; }

        public string NhungDieuCanLuuY { get; set; }

        public string BNSuDungBHYT { get; set; }

        public string ThongTinVeGiaDV { get; set; }

        public int Ngay { get; set; }

        public int Thang { get; set; }

        public int Nam { get; set; }

        public string HoTenBacSi { get; set; }

        public string ChanDoanRaVien { get; set; }

        public string ChanDoanVaoVien { get; set; }

        public DateTime? NgayVaoVien { get; set; }

        public DateTime? NgayRaVien { get; set; }
        public LoaiBenhAn LoaiBenhAn { get; set; }
        public string SoBenhAn { get; set; }
        public string ChanDoanChinh { get; set; }
        public string ChanDoanNhapVien { get; set; }
        public List<string> ChanDoanKemTheo { get; set; }
        public long?  YeuCauKhamBenhId { get; set; }
    }
    public class InPhieuSangLocDinhDuongVo
    { 
        public string KhoaDangIn { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string MaTN { get; set; }
        public string HoTen { get; set; }
        public string NgayThangNamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string ChanDoan { get; set; }
        public string CanNang { get; set; }
        public string SangLocDinhDuong { get; set; }
        public string NguyCoDinhDuong { get; set; }
        public string KeHoachCanThiepDinhDuong { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string BacSyDieuTri { get; set; }
        public string DiemTong { get; set; }
        public string CheDoAnUongDisplay { get; set; }
        public string DuongMieng { get; set; }
        public string OngThong { get; set; }
        public string TinhMach { get; set; }
        public string Khong { get; set; }
        public string Co { get; set; }
        public string TaiDanhGiaBaNgay { get; set; }
        public string TaiDanhGiaBayNgay { get; set; }
    }
}