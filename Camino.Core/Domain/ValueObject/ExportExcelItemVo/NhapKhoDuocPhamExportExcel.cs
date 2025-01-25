using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class NhapKhoDuocPhamExportExcel
    {
        public NhapKhoDuocPhamExportExcel()
        {
            NhapKhoDuocPhamExportExcelChild = new List<NhapKhoDuocPhamExportExcelChild>();
        }

        [Width(50)]
        public string TenKho { get; set; }
        //[Width(30)]
        //public string SoChungTu { get; set; }
        //[Width(30)]
        //public string TenLoaiNhapKho { get; set; }
        //[Width(30)]
        //public string TenNguoiGiao { get; set; }
        //[Width(30)]
        //public string TenNguoiNhap { get; set; }
        [Width(50)]
        public string NgayHoaDonDisplay { get; set; }
        public long Id { get; set; }
        public List<NhapKhoDuocPhamExportExcelChild> NhapKhoDuocPhamExportExcelChild { get; set; }

        [Width(30)]
        public string SoPhieu { get; set; }

        [Width(30)]
        public string SoChungTu { get; set; }

        public long NguoiNhapId { get; set; }

        [Width(30)]
        public string TenNguoiNhap { get; set; }

        public Enums.LoaiNguoiGiaoNhan LoaiNguoiGiao { get; set; }

        [Width(30)]
        public string LoaiNguoiGiaoDisplay
        {
            get { return LoaiNguoiGiao.GetDescription(); }
        }

        public long? NguoiGiaoId { get; set; }

        [Width(30)]
        public string TenNguoiGiao { get; set; }

        public DateTime? NgayNhap { get; set; }

        [Width(30)]
        public string NgayNhapDisplay
        {
            get { return NgayNhap != null ? (NgayNhap ?? DateTime.Now).ApplyFormatDateTime() : ""; }
        }

        public bool? DuocKeToanDuyet { get; set; }

        [Width(30)]
        public string TinhTrangDisplay
        {
            get { return DuocKeToanDuyet != null ? (DuocKeToanDuyet == true ? "Đã duyệt" : "Từ chối duyệt") : "Đang chờ duyệt"; }
        }

        [Width(30)]
        public string NguoiDuyet { get; set; }

        public DateTime? NgayDuyet { get; set; }

        [Width(30)]
        public string NgayDuyetDisplay
        {
            get { return NgayDuyet != null ? (NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : ""; }
        }
    }

    public class NhapKhoDuocPhamExportExcelChild
    {
        //[TitleGridChild("Dược Phẩm")]
        //public string TenDuocPham { get; set; }
        //[TitleGridChild("Hợp Đồng Thầu")]
        //public string TenHopDongThau { get; set; }
        //[TitleGridChild("Số Lô")]
        //public string SoLo { get; set; }
        //[TitleGridChild("Chất Lượng")]
        //public string DatChatLuongText { get; set; }
        //[TitleGridChild("Hạn Sử Dụng")]
        //public string HanSuDungText { get; set; }
        //[TitleGridChild("Số Lượng Nhập")]
        //public string TextSoLuongNhap { get; set; }
        //[TitleGridChild("Đơn Giá Nhập")]
        //public string TextDonGiaNhap { get; set; }
        //[TitleGridChild("Mã Vạch")]
        //public string MaVach { get; set; }
        //[TitleGridChild("Vị Trí")]
        //public string TenViTri { get; set; }

        [TitleGridChild("Dược Phẩm")]
        [Width(30)]
        public string TenDuocPham { get; set; }
        [TitleGridChild("HĐ Thầu")]
        [Width(30)]
        public string TenHDThau { get; set; }
        //public bool LaDuocPhamBHYT { get; set; }
        [TitleGridChild("Loại")]
        [Width(30)]
        public string Loai {  get; set;  }
        [Group]
        public string Nhom { get; set; }
        [TitleGridChild("Số Lô")]
        [Width(30)]
        public string SoLo { get; set; }
        //public DateTime HanSuDung { get; set; }
        [TitleGridChild("Hạn Sử Dụng")]
        [Width(30)]
        public string HanSuDungDisplay
        {
            get; set; 
        }
        [TitleGridChild("Mã Vạch")]
        [Width(30)]
        public string MaVach { get; set; }
        [TitleGridChild("SL")]
        [Width(30)]
        public double SL { get; set; }
        [TitleGridChild("Giá Nhập")]
        [Width(30)]
        [TextAlign("right")]
        public decimal GiaNhap { get; set; }
        [TitleGridChild("VAT(%)")]
        [Width(30)]
        public int VAT { get; set; }

        [TitleGridChild("TL BH Thanh Toán(%)")]
        [Width(30)]
        public int? TiLeBHYTThanhToan { get; set; }
        //[TitleGridChild("Vị Trí")]
        //[Width(30)]
        //public string ViTri { get; set; }
    }
}
