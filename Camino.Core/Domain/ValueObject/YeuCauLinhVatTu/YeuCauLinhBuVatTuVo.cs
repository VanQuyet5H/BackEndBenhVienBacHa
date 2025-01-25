using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.YeuCauLinhVatTu
{
    public class YeuCauLinhBuVatTuVo : GridItem
    {
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
    }

    public class YeuCauLinhVatTuBuGridChildVo : GridItem
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
