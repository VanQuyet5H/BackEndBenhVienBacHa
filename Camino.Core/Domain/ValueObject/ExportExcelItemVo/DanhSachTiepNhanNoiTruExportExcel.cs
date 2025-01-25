using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DanhSachTiepNhanNoiTruExportExcel
    {
        [Width(20)]
        public string MaTiepNhan { get; set; }
        [Width(20)]
        public string MaBenhNhan { get; set; }
        [Width(30)]
        public string HoTen { get; set; }
        [Width(20)]
        public string GioiTinh { get; set; }
        [Width(30)]
        public string KhoaNhapVien { get; set; }
        [Width(30)]
        public string ThoiGianTiepNhanDisplay { get; set; }
        [Width(20)]
        public string SoBenhAn { get; set; }
        [Width(30)]
        public string NoiChiDinh { get; set; }
        [Width(40)]
        public string ChanDoan { get; set; }
        [Width(15)]
        public string DoiTuong { get; set; }

        [Width(15)]
        public string LaCapCuu => CapCuu ? "Có" : "Không";
        [Width(20)]
        public string TenTrangThai { get; set; }


        public bool CapCuu { get; set; }
    }
}
