using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class XuatKhoVatTuExportExcel
    {
        public XuatKhoVatTuExportExcel()
        {
            XuatKhoVatTuExportExcelChild = new List<XuatKhoVatTuExportExcelChild>();
        }
        [Width(30)]
        public string KhoNhap { get; set; }
        [Width(30)]
        public string KhoXuat { get; set; }
        [Width(30)]
        public string SoPhieu { get; set; }
        //[Width(30)]
        //public string LoaiXuatKho { get; set; }
        [Width(30)]
        public string LyDoXuatKho { get; set; }
        [Width(30)]
        public string NguoiNhan { get; set; }
        [Width(30)]
        public string NguoiXuat { get; set; }
        [Width(30)]
        public string NgayXuatDisplay { get; set; }
        public long Id { get; set; }
        public List<XuatKhoVatTuExportExcelChild> XuatKhoVatTuExportExcelChild { get; set; }
    }

    public class XuatKhoVatTuExportExcelChild
    {
        [Group]
        public string LoaiSuDung { get; set; }
        [TitleGridChild("Vật Tư")]
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
