using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class NhapKhoQuaTangMarketingExcel
    {
        [Width(30)]
        public string SoPhieu { get; set; }
        [Width(30)]
        public string SoChungTu { get; set; }
        [Width(30)]
        public string LoaiNguoiGiaoString { get; set; }
        [Width(30)]
        public string TenNguoiGiao { get; set; }
        [Width(30)]
        public string NhanVienNhap { get; set; }
        [Width(30)]
        public string NgayNhapDisplay { get; set; }
    }
}
