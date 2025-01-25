namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class NhaThauExportExcel
    {
        [Width(85)]
        public string Ten { get; set; }

        [Width(117)]
        public string DiaChi { get; set; }

        public string MaSoThue { get; set; }

        [Width(25)]
        public string TaiKhoanNganHang { get; set; }

        public string NguoiDaiDien { get; set; }

        public string NguoiLienHe { get; set; }

        [Width(25)]
        public string SoDienThoaiLienHe { get; set; }

        [Width(21)]
        public string EmailLienHe { get; set; }
    }
}
