using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class ExportDuocPhamBenhVienExportExcel
    {
        [Width(30)]
        public string Ma { get; set; }
        [Width(45)]
        public string Ten { get; set; }
        [Width(30)]
        public string SoDangKy { get; set; }
        [Width(30)]
        public string MaHoatChat { get; set; }
        [Width(40)]
        public string HoatChat { get; set; }
        [Width(30)]
        public string HamLuong { get; set; }
        [Width(30)]
        public string TenDuongDung { get; set; }
        [Width(45)]
        public string QuyCach { get; set; }
        [Width(20)]
        public string TenDonViTinh { get; set; }
        [Width(40)]
        public string TenLoaiThuocHoacHoatChat { get; set; }
       
    }
}
