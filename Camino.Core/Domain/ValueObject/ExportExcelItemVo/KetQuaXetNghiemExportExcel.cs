using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class KetQuaXetNghiemExportExcel : GridItem
    {
        public KetQuaXetNghiemExportExcel()
        {
            KetQuaXetNghiemExportExcelChild = new List<KetQuaXetNghiemExportExcelChild>();
        }

        public string BarCode { get; set; }

        public string MaTN { get; set; }

        [Width(35)]
        public string MaBN { get; set; }

        [Width(25)]
        public string HoTen { get; set; }

        public string GioiTinh { get; set; }

        public string NamSinh { get; set; }

        public string DiaChi { get; set; }


        [Width(25)]
        public string NguoiThucHien { get; set; }

        public string NguoiDuyetKQ { get; set; }

        [Width(25)]
        public string NgayDuyetKQDisplay { get; set; }

        public string NgayThucHienDisplay { get; set; }
        public int? TrangThai { get; set; }
        public string TrangThaiDisplay { get; set; }

        public List<KetQuaXetNghiemExportExcelChild> KetQuaXetNghiemExportExcelChild { get; set; }
    }

    public class KetQuaXetNghiemExportExcelChild
    {
        [Group]
        public string TenNhomDichVuBenhVien { get; set; }

        [TitleGridChild("Mã DV")]
        public string MaDichVu { get; set; }

        [TitleGridChild("Tên DV")]
        public string TenDichVu { get; set; }

        [TitleGridChild("Thời Gian Chỉ Định")]
        public string ThoiGianChiDinhDisplay { get; set; }

        [TitleGridChild("Người Chỉ Định")]
        public string NguoiChiDinh { get; set; }

        [TitleGridChild("Bệnh Phẩm")]
        public string BenhPham { get; set; }

        [TitleGridChild("Loại Mẫu")]
        public string LoaiMau { get; set; }
    }
}
