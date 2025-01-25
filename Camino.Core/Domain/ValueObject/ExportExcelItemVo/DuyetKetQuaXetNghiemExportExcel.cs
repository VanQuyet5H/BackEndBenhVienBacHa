using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DuyetKetQuaXetNghiemExportExcel : GridItem
    {
        public DuyetKetQuaXetNghiemExportExcel()
        {
            DuyetKetQuaXetNghiemExportExcelChild = new List<DuyetKetQuaXetNghiemExportExcelChild>();
        }

        public string Barcode { get; set; }

        public string MaTn { get; set; }

        [Width(35)]
        public string MaBn { get; set; }

        [Width(25)]
        public string HoTen { get; set; }

        public string GioiTinhDisplay { get; set; }

        public int? NamSinh { get; set; }

        public string DiaChi { get; set; }

        public string NguoiThucHien { get; set; }

        [Width(25)]
        public string NgayThucHienDisplay { get; set; }

        public string NguoiDuyetKq { get; set; }

        [Width(25)]
        public string NgayDuyetKqDisplay { get; set; }

        public string TrangThaiDisplay { get; set; }

        public List<DuyetKetQuaXetNghiemExportExcelChild> DuyetKetQuaXetNghiemExportExcelChild { get; set; }
    }

    public class DuyetKetQuaXetNghiemExportExcelChild : GridItem
    {
        [Group]
        public string NhomXetNghiemDisplay { get; set; }

        [TitleGridChild("Mã DV")]
        public string MaDv { get; set; }
        
        [TitleGridChild("Tên DV")]
        public string TenDv { get; set; }
        
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
