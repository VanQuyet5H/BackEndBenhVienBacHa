namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class CanhBaoTonKhoExportExcel
    {
        public string DuocPham { get; set; }
        public string HoatChat { get; set; }
        public string HamLuong { get; set; }
        public string PhanLoai { get; set; }
        public string DonViTinhName { get; set; }
        public double SoLuongTon { get; set; }
        public string CanhBao { get; set; }

        //BVHD-3912
        public string MaDuocPham { get; set; }
    }
}
