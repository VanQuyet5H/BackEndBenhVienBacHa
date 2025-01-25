using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham
{
    public class YeuCauLinhDuocPhamTrucTiepGridParentVo: GridItem
    {
        public int STT { get; set; }
        public string KeyId { get; set; }
        public long YeuCauLinhDuocPhamId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public bool LaBHYT { get; set; }
        public string TenDuocPham { get; set; }
        public string Nhom
        {
            get { return LaBHYT ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT"; }
        }

        public string NongDoHamLuong { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public string DonViTinh { get; set; }
        public string HangSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public double SoLuongTon { get; set; }
        public double SoLuongTonTheoDuocPham { get; set; }
        public double SoLuongYeuCau { get; set; }
        public long? KhoLinhId { get; set; }
        public long? PhongLinhVeId { get; set; }

        public string DichVuKham { get; set; }
        public string BacSiKeToa { get; set; }
        public string NgayKe { get; set; }
        public string HighLightClass => (SoLuongTonTheoDuocPham < SoLuongYeuCau && KhongHighLight != true) ? "bg-row-lightRed" : "";
        public DateTime NgayDieuTri { get; set; }
        public string NgayDieuTriString => NgayDieuTri.ApplyFormatDateTimeSACH();

        public bool? KhongHighLight { get; set; }
    }

    public class YeuCauLinhDuocPhamTrucTiepGridChildVo : GridItem
    {
        public int STT { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public string DichVuKham { get; set; }
        public string BacSiKeToa { get; set; }
        public string NgayKe { get; set; }
        public double SoLuong { get; set; }
        public long BenhNhanId { get; set; }
         public long? KhoLinhId { get; set; }
        public long? PhongLinhVeId { get; set; }
        public string KhoLinh { get; set; }
        public string PhongLinhVe { get; set; }
        public bool? TinhTrangTon { get; set; }

        public double SoLuongTonTheoDichVu { get; set; }
        public bool KhongDuTon => SoLuongTonTheoDichVu < SoLuong;
        public string HighLightClass { get; set; }

    }
    public class DSCanLinhDuocPhamTrucTiepGridParentVo : GridItem
    {
        public int STT { get; set; }
        public string KeyId { get; set; }
        public long YeuCauLinhDuocPhamId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public bool LaBHYT { get; set; }
        public string TenDuocPham { get; set; }
        public string Nhom
        {
            get { return LaBHYT ? "Dược Phẩm BHYT" : "Dược Phẩm Không BHYT"; }
        }

        public string NongDoHamLuong { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public string DonViTinh { get; set; }
        public string HangSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public double SoLuongTon { get; set; }
        public double SoLuongTonTheoDuocPham { get; set; }
        public double SoLuongYeuCau { get; set; }
        public long? KhoLinhId { get; set; }
        public long? PhongLinhVeId { get; set; }

        public string DichVuKham { get; set; }
        public string BacSiKeToa { get; set; }
        public string NgayKe { get; set; }
        public string HighLightClass => SoLuongTon < SoLuongYeuCau ? "bg-row-lightRed" : "";


    }
}
