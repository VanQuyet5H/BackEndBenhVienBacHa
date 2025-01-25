using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class NhapKhoKSNKExportExcel
    {
        public NhapKhoKSNKExportExcel()
        {
            NhapKhoKSNKExportExcelChild = new List<NhapKhoKSNKExportExcelChild>();
        }

        public long Id { get; set; }
        public List<NhapKhoKSNKExportExcelChild> NhapKhoKSNKExportExcelChild { get; set; }

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

        [Width(300)]
        public string NguoiDuyet { get; set; }

        public DateTime? NgayDuyet { get; set; }

        [Width(30)]
        public string NgayDuyetDisplay
        {
            get { return NgayDuyet != null ? (NgayDuyet ?? DateTime.Now).ApplyFormatDateTime() : ""; }
        }
        [Width(30)]
        public string NgayHoaDonDisplay { get; set; }
        [Width(30)]
        public string TenKho { get; set; }

    }

    public class NhapKhoKSNKExportExcelChild
    {

        [TitleGridChild("Vật Tư")]
        [Width(30)]
        public string TenVatTu { get; set; }
        [TitleGridChild("HĐ Thầu")]
        [Width(30)]
        public string TenHDThau { get; set; }
        //public bool LaDuocPhamBHYT { get; set; }
        [TitleGridChild("Loại")]
        [Width(30)]
        public string Loai { get; set; }
        //[TitleGridChild("Nhóm")]
        //[Width(30)]
        //public string Nhom { get; set; }
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
        //public int VAT { get; set; }
        //[TitleGridChild("Vị Trí")]
        //[Width(30)]
        //public string ViTri { get; set; }
        [Group]
        public string LoaiSuDung { get; set; }

        [TitleGridChild("TL BH Thanh Toán(%)")]
        [Width(30)]
        public int? TiLeBHYTThanhToan { get; set; }
    }
}
