using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class VatTuTonKhoCanhBaoExportExcel
    {
        [Width(55)]
        public string TenVatTu { get; set; }
        [Width(20)]
        public string DonViTinh { get; set; }
        [Width(20)]
        public double? SoLuongTon { get; set; }
        [Width(20)]
        public string CanhBao { get; set; }
    }
}
