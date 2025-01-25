using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class XuatKhoDuocPhanExportExcel
    {
        public XuatKhoDuocPhanExportExcel()
        {
            XuatKhoDuocPhanExportExcelChild= new List<XuatKhoDuocPhanExportExcelChild>();
        }
        [Width(30)]
        public string KhoDuocPhamNhap { get; set; }
        [Width(30)]
        public string KhoDuocPhamXuat { get; set; }
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
        public List<XuatKhoDuocPhanExportExcelChild> XuatKhoDuocPhanExportExcelChild { get; set; }
    }

    public class XuatKhoDuocPhanExportExcelChild
    {
        [Group]
        public string Nhom { get; set; }
        [TitleGridChild("Dược Phẩm")]
        public string DuocPham { get; set; }
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