using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class KhamBenhDangKhamExportExcel
    {
        [Group]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        [Width(40)]
        public string Khoa { get; set; }

        [Width(28)]
        public string Phong { get; set; }
        [Width(28)]
        public string TenPhongBenhVien { get; set; }
        [Width(28)]
        public string BacSiDangKham { get; set; }
        [Width(28)]
        public string BenhNhanDangKham { get; set; }
        [Width(28)]
        public int SoLuongBenhNhan { get; set; }
    }
}
