using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.TonKhos
{
    public class VatTuDaHetHan
    {
        public class VatTuDaHetHanGridVo : GridItem
        {
            public string TenKho { get; set; }
            public long? VitriId { get; set; }
            public long? KhoId { get; set; }
            public long VatTuBenhVienId { get; set; }
            public string TenVatTu { get; set; }
            public string DonViTinh { get; set; }
            public string ViTri { get; set; }
            public DateTime? NgayHetHan { get; set; }
            public string NgayHetHanHienThi { get; set; }
            public double SoLuongTon { get; set; }
            public string SoLuongTonFormat => SoLuongTon.ApplyNumber();
            public decimal DonGiaNhap { get; set; }
            public string  MaVatTu { get; set; }
            public string SoLo { get; set; }
            public double ThanhTien => SoLuongTon != 0 && DonGiaNhap != 0 ? (SoLuongTon * Convert.ToDouble(DonGiaNhap)).MathRoundNumber(2) : 0;
        }

        public class VatTuDaHetHanSearchJson
        {
            public long? KhoId { get; set; }
            public string SearchString { get; set; }
        }

        public class InVatTuDaHetHan
        {
            public long KhoId { get; set; }
            public string SearchString { get; set; }
            public bool Header { get; set; }
        }

        public class VatTuDaHetHanData
        {
            public string Header { get; set; }
            public string TemplateVatTu { get; set; }
            public string Ngay { get; set; }
            public string Thang { get; set; }
            public string Nam { get; set; }
        }

        public class VatTuDaHetHanExportExcel
        {
            [Width(40)]
            public string TenKho { get; set; }
            [Width(40)]
            public string TenVatTu { get; set; }
            [Width(20)]
            public string DonViTinh { get; set; }
            [Width(20)]
            public string ViTri { get; set; }
            [Width(20)]
            public double SoLuongTon { get; set; }
            [Width(20)]
            public string NgayHetHanHienThi { get; set; }
            [Width(20)]
            public string MaVatTu { get; set; }
            [Width(20)]
            public string SoLo { get; set; }
            [Width(20)]
            public string DonGiaNhap { get; set; }
            [Width(20)]
            public string ThanhTien { get; set; }
        }
    }
}
