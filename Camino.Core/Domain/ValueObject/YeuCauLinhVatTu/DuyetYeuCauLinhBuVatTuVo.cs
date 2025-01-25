using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.YeuCauLinhVatTu
{
    public class YeuCauLinhBuVatTuGridParentVo : GridItem
    {
        public int STT { get; set; }
        public string KeyId { get; set; }
        public long YeuCauLinhVatTuId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public bool LaBHYT { get; set; }
        public string TenVatTu { get; set; }
        public string Nhom { get; set; }
        public string NongDoHamLuong { get; set; }
        public string HoatChat { get; set; }
        public string DuongDung { get; set; }
        public string DonViTinh { get; set; }
        public string HangSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public double SoLuongTon { get; set; }
        public double SoLuongCanBu { get; set; }
        public string YeuCauLinhVatTuIdstring { get; set; }

        //BVHD-3803
        public string HighLightClass => (SoLuongTon < SoLuongCanBu && KhongHighLight != true) ? "bg-row-lightRed" : "";
        public bool? KhongHighLight { get; set; } = true;
    }

    public class YeuCauLinhBuVatTuGridChildVo : GridItem
    {
        public int STT { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public string DichVuKham { get; set; }
        public string BacSiKeToa { get; set; }
        public string NgayKe { get; set; }
        public double SoLuong { get; set; }
    }
}
