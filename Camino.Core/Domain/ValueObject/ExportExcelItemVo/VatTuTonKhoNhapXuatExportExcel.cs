using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class VatTuTonKhoNhapXuatExportExcel
    {
        [Group]
        [Width(40)]
        public string TenNhomVatTu { get; set; }
        [Width(40)]
        public string Ma { get; set; }
        [Width(65)]
        public string TenVatTu { get; set; }
        [Width(20)]
        public string DonViTinh { get; set; }
        [Width(20)]
        public double TonDauKy { get; set; }
        [Width(20)]
        public double NhapTrongKy { get; set; }
        [Width(20)]
        public double XuatTrongKy { get; set; }
        [Width(20)]
        public double TonCuoiKy { get; set; }
    }
}
