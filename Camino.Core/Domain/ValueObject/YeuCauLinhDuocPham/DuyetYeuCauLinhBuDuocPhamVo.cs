using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.YeuCauLinhDuocPham
{
    public class YeuCauLinhDuocPhamBuGridParentVo : GridItem
    {
        public int STT { get; set; }
        public string KeyId { get; set; }
        public long YeuCauLinhDuocPhamId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public bool LaBHYT { get; set; }
        public string TenDuocPham { get; set; }
        public string Nhom
        {
            get { return LaBHYT ? "Thuốc BHYT" : "Thuốc Không BHYT"; }
        }

        public string NongDoHamLuong { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public string DonViTinh { get; set; }
        public string HangSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public double SoLuongTon { get; set; }
        public double SoLuongCanBu { get; set; }
        public string YeuCauLinhDuocPhamIdstring { get; set; }
        public long? KhoLinhId { get; set; }
        public long? KhoBuId { get; set; }
        public double? SoLuongYeuCau { get; set; }
        public bool? KhongLinhBu { get; set; }
        public double? SoLuongDaBu { get; set; }
        public double? SLYeuCauLinhThucTe { get; set; }
        public double? SLYeuCauLinhThucTeMax => SLYeuCauLinhThucTe;
        public bool CheckBox { get; set; }

        //BVHD-3806
        public string HighLightClass => (SoLuongTon < SoLuongCanBu && KhongHighLight != true) ? "bg-row-lightRed" : "";
        public bool? KhongHighLight { get; set; } = true;
        public DateTime NgayChiDinh { get; set; }
    }

    public class YeuCauLinhDuocPhamBuGridChildVo : GridItem
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
}
