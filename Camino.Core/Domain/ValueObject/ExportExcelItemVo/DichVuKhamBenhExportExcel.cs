using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DichVuKhamBenhExportExcel
    {
        public DichVuKhamBenhExportExcel()
        {
            DichVuKhamBenhExportExcelChild = new List<DichVuKhamBenhExportExcelChild>();
        }

        [Width(30)]
        public string Ma { get; set; }
        [Width(25)]
        public string MaTT37 { get; set; }
        [Width(60)]
        public string Ten { get; set; }
        [Width(45)]
        public string TenKhoa { get; set; }
        [Width(25)]
        public string TenHangBenhVien { get; set; }
        [Width(20)]
        public string MoTa { get; set; }
        public long Id { get; set; }
        public List<DichVuKhamBenhExportExcelChild> DichVuKhamBenhExportExcelChild { get; set; }
    }

    public class DichVuKhamBenhExportExcelChild
    {
        [TitleGridChild("Giá")]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string GiaFormat { get; set; }
        [TitleGridChild("Từ ngày")]
        public string TuNgayFormat { get; set; }
        [TitleGridChild("Đến ngày")]
        public string DenNgayFormat { get; set; }
        [TitleGridChild("Mô tả")]
        public string MoTa { get; set; }
        [TitleGridChild("Hiệu lực")]
        public string ActiveName { get; set; }
    }
}
