using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class CauHinhNguoiDuyetTheoNhomDichVuExportExcel
    {
        [Width(40)]
        public string TenNhanVienDuyet { get; set; }
        [Width(40)]
        public string TenNhomDichVuBenhVien { get; set; }
    }
}
