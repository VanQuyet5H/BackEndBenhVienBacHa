using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.QuayThuoc
{
    public class QuayThuocGridVo : QueryInfo
    {
        public string MaBenhNhan { get; set; }
        public string MaTiepNhan { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string KiemTraThanhToan { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
    }
    public class DateSearch : GridItem
    {
        public DateTime? CreatedOnitem { get; set; }

    }
    public class ThongTinBenhNhanGridVo : GridItem
    {
        public long BenhNhanId { get; set; }
        public string MaBN { get; set; }
        public string MaTN { get; set; }
        public string HoTen { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public string NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhHienThi { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string SoDienThoaiDisPlay { get; set; }
        public string DoiTuong { get; set; }
        public EnumLoaiDonThuoc DoiTuongEnum { get; set; }
        public string TrangThaiHienThi { get; set; }
        public TrangThaiThanhToan TrangThai { get; set; }
        public string SoTien { get; set; }
        public decimal? SoTienNumber { get; set; }
        public string Email { get; set; }
        public bool DonThuocThanhToanExits { get; set; }
        public long? PhuongXaId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? TinhThanhId { get; set; }
        public string DiaChiDayDu { get; set; }
        public List<DateTime> CreatedOn { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public bool IsDisable { get; set; }

        //BVHD-3941
        public bool? CoBaoHiemTuNhan { get; set; }
        public string TenCongTyBaoHiemTuNhan { get; set; }
        public List<ThongTinDonThuocVo> ThongTinDonThuocVos { get; set; }
        public List<ThongTinDonVTYTVo> ThongTinDonVTYTVos { get; set; }
    }

    public class ThongTinDonThuocVo
    {
        public long Id { get; set; }
        public Enums.EnumLoaiDonThuoc LoaiDonThuoc { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public Enums.TrangThaiDonThuocThanhToan TrangThai { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
    public class ThongTinDonVTYTVo
    {
        public long Id { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public Enums.TrangThaiDonVTYTThanhToan TrangThai { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class SettingPhieuThu
    {
        public long Id { get; set; }
        public string Html { get; set; }
        public string TenFile { get; set; }

        public string PageSize { get; set; } //A0,A1,A2,A3,A4,A5
        public string PageOrientation { get; set; } //Landscape,Portrait
    }

    public class CongTyBaoHiemTuNhanGridVo : GridItem
    {
        public long CongNoId { get; set; }
        public string TenCongTy { get; set; }
        public string SoThe { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string TuNgayHienThi { get; set; }
        public string DenNgayHienThi { get; set; }
        public string DienThoai { get; set; }
        public string DiaChi { get; set; }
        public int SoTien { get; set; }
    }
    public class DonThuocThanhToanGridVo : GridItem
    {
        public long? BenhNhanId { get; set; }
        public string MaBN { get; set; }
        public DateTime NgayThu { get; set; }
        public string MaTN { get; set; }
        public string HoTen { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public int? NamSinh { get; set; }
        public string GioiTinhHienThi { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string DoiTuong { get; set; }
        public Enums.TrangThaiDonThuocThanhToan TrangThaiDonThuocThanhToan { get; set; }
        public EnumLoaiDonThuoc DoiTuongEnum { get; set; }

        public TrangThaiThanhToan TrangThai { get; set; }
        public string TrangThaiHienThi => TrangThai.GetDescription();
        public string TrangThaiHienThiString => TrangThai.GetDescription();
        public decimal TongGiaTriDonThuoc { get; set; }
        public decimal SoTienChoThanhToan { get; set; }
        public string TongGiaTriDonThuocString => TongGiaTriDonThuoc.ApplyFormatMoneyVND();
        public string SoTienChoThanhToanString => SoTienChoThanhToan.ApplyFormatMoneyVND();
        public string LoaiDonThuoc { get; set; }
        public bool IsDisable { get; set; }
        public List<DateTime> CreatedOn { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public List<DonThuocCuaBenhNhanGridVo> ListChilDonThuocTrongNgay { get; set; }
    }
    public class ToaThuocCuVO : GridItem
    {
        public string DVKham { get; set; }
        public string DVKhamDiacritics => DVKham.RemoveDiacritics();
        public long DVKhamId { get; set; }
        public string ChuanDoan { get; set; }
        public string ChuanDoanDiacritics => ChuanDoan.RemoveDiacritics();
        public long ChuanDoanId { get; set; }
        public string BacSiKham { get; set; }
        public string BacSiKhamDiacritics => BacSiKham.RemoveDiacritics();
        public long BacSiKhamId { get; set; }
        public string NgayKham { get; set; }

        public DateTime NgayKhamDate { get; set; }
    }
    public class ChiTietThuocToaThuocCuVO : GridItem
    {
        public string TenThuoc { get; set; }
        public string HoatChat { get; set; }
        public string DVT { get; set; }
        public string SoLuong { get; set; }
        public bool LaDuocPhamBenhVien { get; set; }
    }
    public class TimKiemThuocToaThuocCuVO
    {
        public string DVKham { get; set; }
        public string ChuanDoan { get; set; }
        public string BacSi { get; set; }
        public string NgayKham { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
    }

    public class QuayThuocBHYTGridVo : QueryInfo
    {
        public string MaBenhNhan { get; set; }
        public string MaTiepNhan { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string KiemTraThanhToan { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public RangeDate RangeDate { get; set; }
    }

    public class RangeDate
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
