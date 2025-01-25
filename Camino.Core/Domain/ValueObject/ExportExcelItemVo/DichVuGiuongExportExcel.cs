using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DichVuGiuongExportExcel
    {
        public DichVuGiuongExportExcel()
        {
            DichVuGiuongExportExcelChild = new List<DichVuGiuongExportExcelChild>();
        }

        [Width(25)]
        public string Ma { get; set; }
        [Width(25)]
        public string MaTT37 { get; set; }
        [Width(130)]
        public string Ten { get; set; }
        [Width(45)]
        public string Khoa { get; set; }
        [Width(25)]
        public string HangBenhVienDisplay { get; set; }
        [Width(20)]
        public string MoTa { get; set; }
        public long Id { get; set; }
        public List<DichVuGiuongExportExcelChild> DichVuGiuongExportExcelChild { get; set; }
    }

    public class DichVuGiuongExportExcelChild
    {
        [TitleGridChild("Giá")]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string GiaDisplay { get; set; }
        [TitleGridChild("Từ ngày")]
        public string TuNgayDisplay { get; set; }
        [TitleGridChild("Đến ngày")]
        public string DenNgayDisplay { get; set; }
        [TitleGridChild("Mô tả")]
        public string MoTa { get; set; }
        [TitleGridChild("Hiệu lực")]
        public string HieuLucDisplay { get; set; }
    }
}
