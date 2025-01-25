using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class KhamBenhDangKhamTheoPhongKhamExportExcel
    {
        [Width(18)]
        public string MaTiepNhan { get; set; }
        [Width(18)]
        public string MaBenhNhan { get; set; }
        [Width(28)]
        public string HoTen { get; set; }
        [Width(15)]
        public int? NamSinh { get; set; }
        [Width(28)]
        public string SoDienThoai { get; set; }
        [Width(28)]
        public string ThoiDiemTiepNhanDisplay { get; set; }
        [Width(25)]
        public string TenTrangThai { get; set; }
    }
}
