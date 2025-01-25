using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class BenhNhanExportExcel
    {
        //public BenhNhanExportExcel()
        //{
        //    BenhNhanExportExcelChild = new List<BenhNhanExportExcelChild>();
        //}
        [Group]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        [Width(40)]
        public string HoTen { get; set; }
        [Width(20.5)]
        public int? NamSinh { get; set; }
        [Width(20.5)]
        public string SoChungMinhThu { get; set; }
        [Width(20.5)]
        public string GioiTinh { get; set; }
        [Width(20.5)]
        public string Email { get; set; }
        [Width(20.5)]
        public string DiaChi { get; set; }
        //public List<BenhNhanExportExcelChild> BenhNhanExportExcelChild { get; set; }
    }

    public class BenhNhanExportExcelChild
    {
        [Group]
        [TitleGridChild("Họ Tên Child")]
        [Width(100)]
        public string HoTen { get; set; }
    }
}