using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class LichSuThuePhongExportExcel
    {
        [Width(20)]
        public string MaYeuCauTiepNhan { get; set; }
        [Width(20)]
        public string MaNB { get; set; }
        [Width(30)]
        public string TenNB { get; set; }
        [Width(20)]
        public string NgaySinhDisplay { get; set; }
        [Width(50)]
        public string DiaChi { get; set; }
        [Width(15)]
        public string DoiTuong { get; set; }
        [Width(50)]
        public string DichVuThue { get; set; }
        [Width(40)]
        public string LoaiPhongThue { get; set; }
        [Width(20)]
        public string BatDauThueDisplay { get; set; }
        [Width(20)]
        public string KetThucThueDisplay { get; set; }
        [Width(30)]
        public string PhongThucHien { get; set; }
        [Width(30)]
        public string BacSiGayMe { get; set; }
        [Width(30)]
        public string PhauThuatVien { get; set; }
    }
}
