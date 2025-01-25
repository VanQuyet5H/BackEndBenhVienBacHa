using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DonThuocTongNgayExportExcel
    {
        [Width(30)]
        public string MaTN { get; set; }

        [Width(23)]
        public string BenhNhanId { get; set; }

        [Width(54)]
        public string HoTen { get; set; }

        [Width(20)]
        public string NamSinh { get; set; }

        [Width(30)]
        public string GioiTinhHienThi { get; set; }

        [Width(110)]
        public string DiaChi { get; set; }

        [Width(30)]
        public string SoDienThoai { get; set; }

        [Width(20)]
        public string DoiTuong { get; set; }

        [Width(30)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string TongGiaTriDonThuocString { get; set; }

        [Width(30)]
         [TextAlign(Constants.TextAlignAttribute.Right)]
        public string SoTienChoThanhToanString { get; set; }

        [Width(30)]
        public string TrangThaiHienThi { get; set; }
    }
}
