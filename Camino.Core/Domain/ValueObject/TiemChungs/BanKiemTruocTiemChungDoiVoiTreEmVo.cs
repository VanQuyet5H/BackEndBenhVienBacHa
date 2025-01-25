using System;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.TiemChungs
{
    public class BanKiemTruocTiemChungDoiVoiTreEmVo
    {
        public string ImageSrc { get; set; }
        public string Barcode { get; set; }
        public string MaTiepNhan { get; set; }
        public string HoTen { get; set; }
        public string GioiTinhNam { get; set; }
        public string GioiTinhNu { get; set; }
        public int Tuoi { get; set; }
        public int? GioSinh { get; set; }
        public int? PhutSinh { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string SinhNgay { get; set; }
        public string DiaChi { get; set; }
        public string HoTenBoMe { get; set; }
        public string DienThoaiBoMe { get; set; }

        public string CanNang { get; set; }
        public string ThanNhiet { get; set; }
        public string TuoiThaiKhiSinh { get; set; }
        public string GroupXetNghiemHbsAgKhong { get; set; }
        public string GroupXetNghiemHbsAgCo { get; set; }
        public string GroupKetQuaHbsAgCoChild { get; set; }
        public string GroupKetQuaHbsAgKhongChild { get; set; }

        public string Group1Khong { get; set; }
        public string Group1Co { get; set; }
        public string Group2Khong { get; set; }
        public string Group2Co { get; set; }
        public string Group3Khong { get; set; }
        public string Group3Co { get; set; }
        public string Group4Khong { get; set; }
        public string Group4Co { get; set; }
        public string Group5Khong { get; set; }
        public string Group5Co { get; set; }
        public string Group6Khong { get; set; }
        public string Group6Co { get; set; }
        public string Group7Khong { get; set; }
        public string Group7Co { get; set; }
        public string Group8Khong { get; set; }
        public string Group8Co { get; set; }
        public string Group8Text { get; set; }
        public string Group9Khong { get; set; }
        public string Group9Co { get; set; }
        public string Group9Text { get; set; }

        public string GroupKhamSangLocChuyenKhoaKhong { get; set; }
        public string GroupKhamSangLocChuyenKhoaCo { get; set; }
        public string GroupChuyenKhoaText { get; set; }
        public string LyDo { get; set; }
        public string KetQua { get; set; }
        public string KetLuan { get; set; }

        public string TenVacxin { get; set; }
        public string KhongCoBatThuong { get; set; }
        public string ChongChiDinh { get; set; }
        public string TamHoan { get; set; }

        public string KhongKhamSangLoc { get; set; }
        public string CoKhamSangLoc { get; set; }
        public string LyDoKhamSangLoc { get; set; }

        public string Hoi { get; set; }
        public string NgayThangHienTai { get; set; }
        public string HoTenBacSi { get; set; }

        //BVHD-3812
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public string CanCuocCongDan { get; set; }
        public string SoDienThoai { get; set; }
        public string NgheNghiep { get; set; }
        public string DonViCongTac { get; set; }

        public string DaTiemMui1GroupKhong { get; set; }
        public string DaTiemMui1GroupCo1 { get; set; }
        public string DaTiemMui1GroupCo1Text { get; set; }
        public string DaTiemMui1GroupCo1DateValue { get; set; }
        public string DaTiemMui1GroupCo1Date => string.IsNullOrEmpty(DaTiemMui1GroupCo1DateValue?.Trim()) ? "" : (DateTime.Parse(DaTiemMui1GroupCo1DateValue).ApplyFormatDate());

        public string DaTiemMui1GroupCo2 { get; set; }
        public string DaTiemMui1GroupCo2Text { get; set; }
        public string DaTiemMui1GroupCo2DateValue { get; set; }
        public string DaTiemMui1GroupCo2Date => string.IsNullOrEmpty(DaTiemMui1GroupCo2DateValue?.Trim()) ? "" : (DateTime.Parse(DaTiemMui1GroupCo2DateValue).ApplyFormatDate());

        public string DaTiemMui1GroupCo3 { get; set; }
        public string DaTiemMui1GroupCo3Text { get; set; }
        public string DaTiemMui1GroupCo3DateValue { get; set; }
        public string DaTiemMui1GroupCo3Date => string.IsNullOrEmpty(DaTiemMui1GroupCo3DateValue?.Trim()) ? "" : (DateTime.Parse(DaTiemMui1GroupCo3DateValue).ApplyFormatDate());

        public string Group313TuanKhong { get; set; }
        public string Group313TuanCo { get; set; }
        public string Group3Hon13TuanKhong { get; set; }
        public string Group3Hon13TuanCo { get; set; }
        public string Group4Text { get; set; }
        public string Group5Text { get; set; }
        
        public string Group9NhietDo { get; set; }
        public string Group9Mach { get; set; }
        public string Group9HuyetAp { get; set; }
        public string Group9NhipTho { get; set; }

        public string Group10Text { get; set; }
        public string Group10Khong { get; set; }
        public string Group10Co { get; set; }

        public string KhongCoBatThuongTiemCovid { get; set; }
        public string ChongChiDinhTiemCovid { get; set; }
        public string TamHoanTiemCovid { get; set; }
        public string ChiDinhCoSoYTeCoDieuKienTiemCovid { get; set; }
        public string NhomThanTrongTiemCovid { get; set; }

        public string LyDoKetLuan { get; set; }

        public DateTime ThoiDiemHienTai => DateTime.Now;
        public string Gio => ThoiDiemHienTai.Hour.ToString("00");
        public string Phut => ThoiDiemHienTai.Minute.ToString("00");
        public int Ngay => ThoiDiemHienTai.Day;
        public int Thang => ThoiDiemHienTai.Month;
        public int Nam => ThoiDiemHienTai.Year;


    }
}