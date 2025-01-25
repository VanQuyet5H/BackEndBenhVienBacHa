using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.KhamDoan
{
    public class HopDongKhamSucKhoeNhanVienViewModel : BaseViewModel
    {
        public long HopDongKhamSucKhoeId { get; set; }
        public long? BenhNhanId { get; set; }
        public int? STTNhanVien { get; set; }
        public string MaBN { get; set; }
        public string MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public string HoTenKhac { get; set; }
        public DateTime? NgayThangNamSinh { get; set; }

        public int? Nam => NgayThangNamSinh?.Year;
        public int? Tuoi => DateTime.Now.Year - NgayThangNamSinh?.Year;
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }

        public string SoDienThoai { get; set; }
        public string SoChungMinhThu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string TenGioiTinh => GioiTinh?.GetDescription();

        public long? NgheNghiepId { get; set; }
        public long? QuocTichId { get; set; }
        public long? DanTocId { get; set; }

        public string DiaChi { get; set; }
        public long? PhuongXaId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? TinhThanhId { get; set; }

        public Enums.EnumNhomMau? NhomMau { get; set; }
        public string TenNhomMau => NhomMau?.GetDescription();

        public Enums.EnumYeuToRh? YeuToRh { get; set; }
        public string TenYeuToRh => YeuToRh?.GetDescription();

        public string Email { get; set; }
        public string TenDonViHoacBoPhan { get; set; }
        public string NhomDoiTuongKhamSucKhoe { get; set; }
        public long? GoiKhamSucKhoeId { get; set; }
        public bool DaLapGiaDinh { get; set; }
        public bool? CoMangThai { get; set; }

        public TinhTrangHonNhan? TinhTrangHonNhan { get; set; }
        public string TenTinhTrangHonNhan => TinhTrangHonNhan?.GetDescription();

        public List<TienSuBenh> TienSuBenhs { get; set; }
        public string GhiChuTienSuBenh { get; set; }

        public string GhiChuDiUngThuoc { get; set; }
        public string DiaChiDayDu { get; set; }

        public DateTime? NgayCapChungMinhThu { get; set; }
        public string NoiCapChungMinhThu { get; set; }
        public DateTime? NgayBatDauLamViec { get; set; }

        public List<NgheCongViecTruocDay> NgheCongViecTruocDays { get; set; }
        public string NgheCongViecTruocDay { get; set; }

        public long? HoKhauTinhThanhId { get; set; }
        public long? HoKhauQuanHuyenId { get; set; }
        public long? HoKhauPhuongXaId { get; set; }
        public string HoKhauDiaChi { get; set; }
        public string TenGoiKhamSucKhoe { get; set; }

        public bool? TinhTrangKham { get; set; }
    }

    public class NgheCongViecTruocDay
    {
        public string CongViec { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }

    public class TienSuBenh
    {
        public long Id { get; set; }
        public long LoaiTienSuId { get; set; }
        public string LoaiTienSu { get; set; }
        public bool? BenhNgheNghiep { get; set; }
        public string TenBenh { get; set; }
        public DateTime? PhatHienNam { get; set; }
    }

    public class DanhSachNhapExcelError
    {
        public string MaGoi { get; set; }
        public string MaNV { get; set; }
        public string TenNhanVien { get; set; }
        public int TotalThanhCong { get; set; }
        public string Error { get; set; }
    }
}
