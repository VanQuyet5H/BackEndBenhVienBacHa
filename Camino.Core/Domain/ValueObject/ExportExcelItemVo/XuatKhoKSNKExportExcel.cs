using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class XuatKhoKSNKExportExcel
    {
        public XuatKhoKSNKExportExcel()
        {
            XuatKhoKSNKExportExcelChild = new List<XuatKhoKSNKVatTuVaDuocPhamExportExcelChild>();
        }
        [Width(30)]
        public string KhoNhap { get; set; }
        [Width(30)]
        public string KhoXuat { get; set; }
        [Width(30)]
        public string SoPhieu { get; set; }

        public LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }

        [Width(30)]
        public string LyDoXuatKho { get; set; }
        [Width(30)]
        public string NguoiNhan { get; set; }
        [Width(30)]
        public string NguoiXuat { get; set; }
        [Width(30)]
        public string NgayXuatDisplay { get; set; }
        public long Id { get; set; }
        public List<XuatKhoKSNKVatTuVaDuocPhamExportExcelChild> XuatKhoKSNKExportExcelChild { get; set; }
    }

    public class XuatKhoKSNKVatTuVaDuocPhamExportExcelChild
    {
        [Group]
        public string LoaiSuDung { get; set; }
        [TitleGridChild("Tên")]
        public string VatTu { get; set; }
        [TitleGridChild("ĐVT")]
        public string DVT { get; set; }
        [TitleGridChild("Loại")]
        public string Loai { get; set; }
        [TitleGridChild("SL Xuất")]
        public string SoLuongXuat { get; set; }
        [TitleGridChild("Số Phiếu")]
        public string SoPhieu { get; set; }
    }
}
