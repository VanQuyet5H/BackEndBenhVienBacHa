using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class GachNoExportExcel
    {
        [Width(20)]
        public string SoChungTu { get; set; }
        [Width(20)]
        public string TenLoaiDoiTuong { get; set; }
        [Width(30)]
        public string LoaiThuChi { get; set; }
        [Width(20)]
        public string NgayChungTuDisplay { get; set; }
        [Width(20)]
        public string TaiKhoan { get; set; }
        [Width(40)]
        public string DienGiai { get; set; }
        [Width(10)]
        public int? VAT { get; set; }
        [Width(40)]
        public string KhoanMucPhi { get; set; }
        [Width(20)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string TienHachToan { get; set; }
        [Width(20)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string TienThueHachToan { get; set; }
        [Width(20)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string TongTienHachToan { get; set; }
        [Width(20)]
        public string SoHoaDon { get; set; }
        [Width(20)]
        public string NgayHoaDonDisplay { get; set; }
        [Width(20)]
        public string MaKhachHang { get; set; }
        [Width(40)]
        public string TenKhachHang { get; set; }
        [Width(20)]
        public string MaSoThue { get; set; }
        [Width(20)]
        public string TenTrangThai { get; set; }
    }
}
