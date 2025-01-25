using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class NhapKhoMauExportExcel
    {
        public NhapKhoMauExportExcel()
        {
            NhapKhoMauExportExcelChild = new List<NhapKhoMauExportExcelChild>();
        }

        [Width(100)]
        public string SoPhieu { get; set; }
        [Width(35)]
        public string SoHoaDon { get; set; }
        [Width(30)]
        public string NgayHoaDonDisplay { get; set; }
        [Width(40)]
        public string NhaCungCap { get; set; }
        [Width(30)]
        public string GhiChu { get; set; }
        [Width(30)]
        public string TenTinhTrang { get; set; }
        [Width(35)]
        public string NguoiNhap { get; set; }
        [Width(30)]
        public string NgayNhapDisplay { get; set; }
        [Width(35)]
        public string NguoiDuyet { get; set; }
        [Width(30)]
        public string NgayDuyetDisplay { get; set; }
        public long Id { get; set; }
        public List<NhapKhoMauExportExcelChild> NhapKhoMauExportExcelChild { get; set; }
    }

    public class NhapKhoMauExportExcelChild
    {
        [TitleGridChild("Mã Túi Máu")]
        public string MaTuiMau { get; set; }
        [TitleGridChild("Chế Phẩm Máu")]
        public string ChePhamMau { get; set; }
        [TitleGridChild("Ngày Sản Xuất")]
        public string NgaySanXuatDisplay { get; set; }
        [TitleGridChild("Hạn Sử Dụng")]
        public string HanSuDungDisplay { get; set; }
        [TitleGridChild("Đơn Giá DV")]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public decimal? DonGiaDichVu { get; set; }
        [TitleGridChild("Đơn Giá BH")]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public decimal? DonGiaBaoHiem { get; set; }
    }
}
