using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;
namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class XacNhanInPhieuSuatAn
    {
        public long YeuCauTiepNhanId { get; set; }
        public long PhieuDieuTriId { get; set; }
        public List<long> DichVuDuocChon { get; set; }
        public string Hosting { get; set; }
    }
    public class InDanhSachSuatAn
    {
        public string TenNhomDichVuBenhVien { get; set; }
        public string TenNoiThucHien { get; set; }
        public string Nhom => TenNhomDichVuBenhVien + "-" + TenNoiThucHien;

        public string Ten { get; set; }
        public string BuaAn { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class PhieuDieuTriSuatAnGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int SoLan { get; set; }
        public string DoiTuong => DoiTuongSuDung.GetDescription();
        public DoiTuongSuDung? DoiTuongSuDung { get; set; }
        public BuaAn? BuaAn { get; set; }
        public string BuaAnDisplay => BuaAn?.GetDescription();        
        public decimal DonGia { get; set; }
        public decimal? ThanhTien => SoLan * DonGia;
        public long? DichVuKyThuatBenhVienId { get; set; }
        public string TenNhanVienChiDinh { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public string ThoiDiemChiDinhDisplay => ThoiDiemChiDinh?.ApplyFormatDateTime();
    }

    public class PhieuDieuTriTruyenMauGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long TheTich { get; set; }
        public EnumNhomMau? NhomMau { get; set; }
        public EnumYeuToRh? YeuToRh { get; set; }
        public string NhomMauDisplay => NhomMau?.GetDescription() + YeuToRh?.GetDescription();
        public string TenNhomMau => NhomMau?.GetDescription() + YeuToRh?.GetDescription();
        public decimal? DonGiaBaoHiem { get; set; }
        public decimal? DonGiaBan { get; set; }
        public decimal? ThanhTien => DonGiaBan * 1;//số lượng là 1
        public EnumTrangThaiYeuCauTruyenMau TrangThai { get; set; }
        public string TrangThaiDisplay => TrangThai.GetDescription();
        public int? ThoiGianBatDauTruyen { get; set; }
        public long MauVaChePhamId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public string TenNhanVienChiDinh { get; set; }
        public bool DaNhapKhoMauChiTiet { get; set; }
        public string ThoiDiemChiDinhDisplay => ThoiDiemChiDinh?.ApplyFormatDateTime();

        // BVHD-3959 
        public bool ChecBoxItem { get; set; }
        public DateTime? ThoiGianDienBien { get; set; }
        public string ThoiGianDienBienDisplayname => ThoiGianDienBien != null ? ThoiGianDienBien.GetValueOrDefault().ApplyFormatDateTime() : "";
    }

    public class MauVaChePhamTemplateVo : GridItem
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long TheTich { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => DonGia * 1; // số lượng là 1
        public string TheTichDisplay => TheTich + "cc";

    }
    public class NhomMauTemplateVo : GridItem
    {
        public string KeyId { get; set; }
        public string DisplayName { get; set; }
    }

    public class PhieuDieuTriTruyenMauVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public long MauVaChePhamId { get; set; }
        public long TheTich { get; set; }
        public EnumNhomMau? NhomMau { get; set; }
        public EnumYeuToRh? YeuToRh { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
    }
    public class XacNhanInPhieuTruyenMau
    {
        public long YeuCauTiepNhanId { get; set; }
        public long PhieuDieuTriId { get; set; }
        public string Hosting { get; set; }
    }
    public class InDanhSachTruyenMau
    {
        public Enums.EnumYeuToRh? YeuToRh { get; set; }
        public string Ten { get; set; }
        public double SoLuong { get; set; }
        public string NhomMau { get; set; }
    }
    public class ApDungThoiGianDienBienTruyenMauVo
    {
        public ApDungThoiGianDienBienTruyenMauVo()
        {
            DataGridDichVuChons = new List<PhieuDieuTriTruyenMauGridVo>();
        }
        public List<PhieuDieuTriTruyenMauGridVo> DataGridDichVuChons { get; set; }

        public DateTime? ThoiGianDienBien { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long PhieuDieuTriId { get; set; }
    }
}
