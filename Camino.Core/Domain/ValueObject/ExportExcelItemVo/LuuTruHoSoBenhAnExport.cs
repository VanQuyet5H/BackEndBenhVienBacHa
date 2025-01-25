using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class LuuTruHoSoBenhAnExport 
    {
        [Width(20)]
        public string ThuTuSap { get; set; }
        [Width(20)]
        public string SoLuuTru { get; set; }
        [Width(35)]
        public string HoTen { get; set; }
        [Width(15)]
        public string GioiTinh { get; set; }
        [Width(15)]
        public string Tuoi { get; set; }
        [Width(30)]
        public string ThoiGianVaoVienString { get; set; }
        [Width(30)]
        public string ThoiGianRaVienString { get; set; }
        [Width(40)]
        public string ChanDoan { get; set; }
        [Width(40)]
        public string ICD { get; set; }
    }
}
