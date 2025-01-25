using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class LichSuBanThuocExportExcel
    {
        [Width(20)]
        public string SoDon { get; set; }

        [Width(30)]
        public string MaTN { get; set; }

        [Width(30)]
        public string MaBN { get; set; }

        [Width(30)]
        public string HoTen { get; set; }

        [Width(10)]
        public string NamSinh { get; set; }
        [Width(10)]
        public string GioiTinhHienThi { get; set; }
        [Width(100)]
        public string DiaChi { get; set; }

        [Width(20)]
        public string SoDienThoai { get; set; }
        [Width(12)]
        public string DoiTuong { get; set; }
     

        [Width(30)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string SoTienThuString { get; set; }

        [Width(40)]
        public string NgayThuStr { get; set; }

        [Width(40)]
        public string ThoiDiemCapPhatThuoc { get; set; }

        
    }
    public class LichSuXuatThuocExportExcel
    {
        [Width(20)]
        public string SoDon { get; set; }

        [Width(30)]
        public string MaTN { get; set; }

        [Width(30)]
        public string MaBN { get; set; }

        [Width(30)]
        public string HoTen { get; set; }
        public string NamSinh { get; set; }
        [Width(10)]
        public string GioiTinhHienThi { get; set; }
        [Width(100)]
        public string DiaChi { get; set; }

        [Width(20)]
        public string SoDienThoai { get; set; }

        [Width(30)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string SoTienThuString { get; set; }

        [Width(18)]
        public string DoiTuong { get; set; }

        [Width(40)]
        public string ThoiDiemCapPhatThuocString { get; set; }


    }

    public class LichSuCapThuocBHYTExportExcel
    {
        [Width(20)]
        public string SoChungTu { get; set; }

        [Width(30)]
        public string MaTN { get; set; }

        [Width(30)]
        public string MaBN { get; set; }

        [Width(30)]
        public string HoTen { get; set; }
        public string NamSinh { get; set; }
        [Width(10)]
        public string GioiTinhHienThi { get; set; }
        [Width(100)]
        public string DiaChi { get; set; }

        [Width(20)]
        public string SoDienThoai { get; set; }

        [Width(30)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string SoTienThuString { get; set; }

        [Width(18)]
        public string DoiTuong { get; set; }

        [Width(40)]
        public string NgayXuatThuocDisplay { get; set; }


    }
}
