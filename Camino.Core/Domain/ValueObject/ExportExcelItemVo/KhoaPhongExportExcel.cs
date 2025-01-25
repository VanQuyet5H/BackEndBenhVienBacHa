using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class KhoaPhongExportExcel
    {
        [Width(50)]
        public string Ten { get; set; }

        [Width(25)]
        public string Ma { get; set; }

        public string CoKhamNgoaiTru { get; set; } //

        public string IsDisabled { get; set; } //

        [Width(25)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string SoTienThuTamUng { get; set; } //

        [Width(113)]
        public string TenKhoa { get; set; }

        [Group]
        public string TenLoaiKhoaPhongDisplayName { get; set; }
    }
}
