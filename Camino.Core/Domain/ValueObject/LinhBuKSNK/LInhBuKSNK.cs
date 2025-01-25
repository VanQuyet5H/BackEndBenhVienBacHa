using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.LinhBuKSNK
{
    public class DanhSachKSNKCanBuGridVo : GridItem
    {
        public long KhoLinhId { get; set; }
        public string KhoLinh { get; set; }
        public long KhoBuId { get; set; }
        public string KhoBu { get; set; }
        public bool LoaiDuocPhamHayVatTu { get; set; }
    }
    public class DanhSachKSNKCanBuQueryInfo : QueryInfo
    {
        public long? KhoLinhId { get; set; }
        public long? KhoBuId { get; set; }

    }
    public class DanhSachKSNKCanBuChiTietQueryInfo : QueryInfo
    {
        public long? KhoLinhId { get; set; }
        public long? KhoBuId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public bool? LaBHYT { get; set; }
        public bool? LoaiDuocPhamHayVatTu { get; set; }
    }
    public class YeuCauLinhBuKSNKVo : GridItem
    {
        public YeuCauLinhBuKSNKVo(){
            YeuCauLinhVatTuInFos = new List<KhongYeuCauLinhBuKSNKVo>();
        }

        public int STT { get; set; }
        public string KeyId { get; set; }
        public long YeuCauLinhVatTuId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public bool LaBHYT { get; set; }
        public string TenVatTu { get; set; }
        public string Nhom { get; set; }
        public string DonViTinh { get; set; }
        public string HangSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public double SoLuongTon { get; set; }
        public double SoLuongCanBu { get; set; }
        public double? SoLuongDaLinhBu { get; set; }
        public string YeuCauLinhVatTuIdstring { get; set; }
        public long? KhoLinhId { get; set; }
        public long? KhoBuId { get; set; }
        public bool? KhongLinhBu { get; set; }
        public bool LoaiDuocPhamHayVatTu { get; set; }
        public List<KhongYeuCauLinhBuKSNKVo> YeuCauLinhVatTuInFos { get; set; }
    }

    public class YeuCauLinhKSNKBuGridChildVo : GridItem
    {
        public int STT { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public string DichVuKham { get; set; }
        public string BacSiKeToa { get; set; }
        public string NgayKe { get; set; }
        public double SoLuong { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public string NgayDieuTriString => NgayDieuTri.ApplyFormatDateTimeSACH();

        //BVHD-3806
        public double SoLuongTonTheoDichVu { get; set; }
        public bool KhongDuTon => SoLuongTonTheoDichVu < SoLuong;
        public string HighLightClass => (KhongDuTon && KhongHighLight != true) ? "bg-row-lightRed" : "";
        public bool? KhongHighLight { get; set; } = true;
    }
    public class KSNKLinhBuCuaBNGridVo : GridItem
    {
        public int? STT { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string DVKham { get; set; }
        public long? BSKeToaId { get; set; }
        public string BSKeToa { get; set; }
        public string NgayKe { get; set; }
        public double? SL { get; set; }
        public double? SLDaBu { get; set; }
        public double? SLYeuCau { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public string NgayDieuTriString => NgayDieuTri.ApplyFormatDateTimeSACH();

    }
    public class YeuCauDuocPhamVatTuBenhVienGridVo : GridItem
    {
        public long? KhoLinhId { get; set; }
        public bool LaDuocPhamVatTuBHYT { get; set; }
        public long DuocPhamVatTuBenhVienId { get; set; }
        public bool LoaiDuocPhamHayVatTu { get; set; }
        public bool LaKhoHanhChinhVaNhomHanhChinh { get; set; }
    }
    public class KhongYeuCauLinhBuKSNKVo 
    {
        public long YeuCauLinhId { get; set; }
        public bool  LoaiDuocPhamHayVatTu { get; set; }
    }
    public class KhongYeuCauLinhBuKSNK
    {
        public KhongYeuCauLinhBuKSNK()
        {
            linhBuVo = new List<KhongYeuCauLinhBuKSNKVo>();
        }
        public List<KhongYeuCauLinhBuKSNKVo> linhBuVo{ get; set; }
}
    public class VatTuThuocNhomHCGridVo
    {
        public long DuocPhamVatTuBenhVienId { get; set; }
        public long KhoId { get; set; }
        public bool? DaHet { get; set; }
        public bool LaDuocPhamVatTuBHYT { get; set; }
        public double SoLuongNhap { get; set; }
        public double SoLuongDaXuat { get; set; }
        public EnumLoaiKhoDuocPham VatTuThuocLoaiKhoHC { get; set; }
        public long VatTuThuocNhomHC { get; set; }
    }
}
