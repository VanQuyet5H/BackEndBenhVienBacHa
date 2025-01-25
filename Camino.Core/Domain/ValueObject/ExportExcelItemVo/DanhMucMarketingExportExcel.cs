using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DanhMucMarketingExportExcel
    {
        public DanhMucMarketingExportExcel()
        {
            DanhMucMarketingExportExcelChild = new List<DanhMucMarketingExportExcelChild>();
        }

        public List<DanhMucMarketingExportExcelChild> DanhMucMarketingExportExcelChild { get; set; }

        public int STT { get; set; }
        [TitleGridChild("Mã BN")]
        [Width(30)]
        public string MaBenhNhan { get; set; }
        [TitleGridChild("Người Bệnh")]
        [Width(30)]
        public string TenBenhNhan { get; set; }
        [TitleGridChild("Năm Sinh")]
        [Width(30)]
        public string NamSinh { get; set; }
        [TitleGridChild("Giới Tính")]
        [Width(30)]
        public string GioiTinh { get; set; }
        public string DienThoai { get; set; }
        [TitleGridChild("Điện Thoại")]
        [Width(30)]
        public string DienThoaiDisplay { get; set; }
        [TitleGridChild("CMND")]
        [Width(30)]
        public string ChungMinhThu { get; set; }
        [TitleGridChild("Địa Chỉ")]
        [Width(30)]
        public string DiaChi { get; set; }
        [TitleGridChild("Ngày Tạo")]
        [Width(30)]
        public string NgayTaoDisplay { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool EnableDeleteButton { get; set; }

        public long Id { get; set; }

    }

    public class DanhMucMarketingExportExcelChild
    {
        [TitleGridChild("Chương Trình Gói Marketing")]
        public string ChuongTrinhGoiMarketing { get; set; }
        [TitleGridChild("Tổng Tiền TT")]
        public string TongTienTTDisplay { get; set; }
        [TitleGridChild("T.Trạng Thanh Toán")]
        public string TrangThaiTT { get; set; }
        [TitleGridChild("T.Trạng Sử Dụng")]
        public string TrangThaiSuDung { get; set; }
        [TitleGridChild("T.Trạng Nhận Quà")]
        public string TrangThaiNhanQua { get; set; }

        [TitleGridChild("Ngày Đăng Ký")]
        public string NgayDangKyDisplay { get; set; }
    }
}
