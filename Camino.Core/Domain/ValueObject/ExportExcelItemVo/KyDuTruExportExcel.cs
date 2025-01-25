using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class KyDuTruExportExcel
    {
        [Width(20)]
        public string TuNgayDisplay { get; set; }
        [Width(30)]
        public string DenNgayDisplay { get; set; }
        [Width(40)]
        public string NhanVienTaoDisplay { get; set; }
        [Width(40)]
        public string ApDung { get; set; }
        [Width(20)]
        public string HieuLucDisplay { get; set; }
        [Width(30)]
        public string NgayTaoDisplay { get; set; }
        [Width(30)]
        public string NgayBatDauLapDisplay { get; set; }
        [Width(30)]
        public string NgayKetThucLapDisplay { get; set; }
    }
}
