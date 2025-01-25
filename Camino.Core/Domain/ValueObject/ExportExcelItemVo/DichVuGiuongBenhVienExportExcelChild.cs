namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DichVuGiuongBenhVienExportExcelChild
    {
        [TitleGridChild("Kiểu Giá")]
        [Group]
        public string KieuGia { get; set; }

        [TitleGridChild("Loại Giá")]
        public string LoaiGia { get; set; }

        [TitleGridChild("Giá")]
        public string GiaHienThi { get; set; }

        [TitleGridChild("TLTT(%)")]
        public string TiLeBaoHiemThanhToan { get; set; }

        [TitleGridChild("Từ ngày")]
        public string TuNgayHienThi { get; set; }

        [TitleGridChild("Đến ngày")]
        public string DenNgayHienThi { get; set; }
    }
}
