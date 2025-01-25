using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class GoiMauDanhSachXetNghiemExportExcel : GridItem
    {
        public GoiMauDanhSachXetNghiemExportExcel()
        {
            GoiMauDanhSachXetNghiemExportExcelChild = new List<GoiMauDanhSachNhomXetNghiemExportExcelChild>();
        }

        [Width(20)]
        public string SoPhieu { get; set; }
        [Width(30)]
        public string NguoiGoiMauDisplay { get; set; }
        [Width(20)]
        public string NgayGoiMauDisplay { get; set; }
        [Width(20)]
        public string SoLuongMau { get; set; }
        [Width(30)]
        public string TinhTrangDisplay { get; set; }
        [Width(20)]
        public string NoiTiepNhan { get; set; }
        [Width(20)]
        public string NguoiNhanMauDisplay { get; set; }
        [Width(20)]
        public string NgayNhanMauDisplay { get; set; }

        public List<GoiMauDanhSachNhomXetNghiemExportExcelChild> GoiMauDanhSachXetNghiemExportExcelChild { get; set; }
    }

    public class GoiMauDanhSachNhomXetNghiemExportExcelChild
    {
        public GoiMauDanhSachNhomXetNghiemExportExcelChild()
        {
            GoiMauDanhSachNhomXetNghiemExportExcelChildChild = new List<GoiMauDanhSachDichVuXetNghiemExportExcelChild>();
        }

        public long PhieuGoiMauXetNghiemId { get; set; }
        public long PhienXetNghiemId { get; set; }

        [TitleGridChild("Xét nghiệm")]
        public string NhomDichVuBenhVienDisplay { get; set; }
        [TitleGridChild("Mã Barcode")]
        public string Barcode { get; set; }
        [TitleGridChild("Loại mẫu")]
        public string LoaiMauXetNghiemDisplay { get; set; }
        [TitleGridChild("Mã TN")]
        public string MaTiepNhan { get; set; }
        [TitleGridChild("Mã BN")]
        public string MaBenhNhan { get; set; }
        [TitleGridChild("Họ tên")]
        public string HoTen { get; set; }
        [TitleGridChild("Năm sinh")]
        public int? NamSinh { get; set; }
        [TitleGridChild("Giới tính")]
        public string GioiTinhDisplay { get; set; }

        public List<GoiMauDanhSachDichVuXetNghiemExportExcelChild> GoiMauDanhSachNhomXetNghiemExportExcelChildChild { get; set; }
    }

    public class GoiMauDanhSachDichVuXetNghiemExportExcelChild
    {
        [TitleGridChild("Mã DV")]
        public string MaDichVu { get; set; }
        [TitleGridChild("Tên DV")]
        public string TenDichVu { get; set; }
        [TitleGridChild("Thời gian chỉ định")]
        public string ThoiGianChiDinhDisplay { get; set; }
        [TitleGridChild("Người chỉ định")]
        public string NguoiChiDinhDisplay { get; set; }
        [TitleGridChild("Bệnh phẩm")]
        public string BenhPham { get; set; }
        [TitleGridChild("Loại mẫu")]
        public string LoaiMauDisplay { get; set; }
    }
}
