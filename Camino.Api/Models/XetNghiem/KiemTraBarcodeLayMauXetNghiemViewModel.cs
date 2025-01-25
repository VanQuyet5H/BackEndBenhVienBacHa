using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.XetNghiem
{
    public class KiemTraBarcodeLayMauXetNghiemViewModel
    {
        public long? YeuCauTiepNhanId { get; set; }
        public string BarcodeNumber { get; set; }
        public string BarcodeString { get; set; }
        public bool IsInBarcode { get; set; }
        public int? SoLuong { get; set; }
        public bool IsCapMoi { get; set; }
        public bool? IsCapBarcodeChoDichVu { get; set; }
    }
}
