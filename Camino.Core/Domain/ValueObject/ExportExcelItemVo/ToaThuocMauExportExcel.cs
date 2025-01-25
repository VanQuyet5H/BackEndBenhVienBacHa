using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class ToaThuocMauExportExcel
    {
        public ToaThuocMauExportExcel()
        {
            ToaThuocMauExportExcelChild = new List<ToaThuocMauExportExcelChild>();
        }

        [Width(20)]
        public string Ten { get; set; }
        [Width(60)]
        public string TenICD { get; set; }
        [Width(20)]
        public string TenTrieuChung { get; set; }
        [Width(140)]
        public string ChuanDoan { get; set; }
        [Width(25)]
        public string TenBacSiKeToa { get; set; }
        [Width(20)]
        public string GhiChu { get; set; }
        public long Id { get; set; }
        public List<ToaThuocMauExportExcelChild> ToaThuocMauExportExcelChild { get; set; }
    }

    public class ToaThuocMauExportExcelChild
    {
        [TitleGridChild("Dược phẩm")]
        public string TenDuocPham { get; set; }
        [TitleGridChild("Số lượng")]
        public double? SoLuong { get; set; }
        [TitleGridChild("Số lượng")]
        public string SoLuongDisplay { get; set; }
        [TitleGridChild("Số ngày dùng")]
        public int? SoNgayDung { get; set; }
        [TitleGridChild("Dùng sáng")]
        public string DungSangDisplay { get; set; }
        [TitleGridChild("Dùng trưa")]
        [Width(20)]
        public string DungTruaDisplay { get; set; }
        [TitleGridChild("Dùng chiều")]
        [Width(20)]
        public string DungChieuDisplay { get; set; }
        [TitleGridChild("Dùng tối")]
        [Width(20)]
        public string DungToiDisplay { get; set; }
    }
}
