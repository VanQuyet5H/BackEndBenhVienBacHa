namespace Camino.Api.Models.HoanTra
{
    public class DuyetHoanTraDuocPhamViewModel : BaseViewModel
    {
        public long NhanVienTraId { get; set; }
        public string TenNhanVienTra { get; set; }
        public long NhanVienNhanId { get; set; }
        public string TenNhanVienNhan { get; set; }
        //public string LyDoHuy { get; set; }
    }
}