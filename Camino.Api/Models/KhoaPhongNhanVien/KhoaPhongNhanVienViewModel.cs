namespace Camino.Api.Models.KhoaPhongNhanVien
{
    public class KhoaPhongNhanVienViewModel : BaseViewModel
    {
        public long KhoaPhongId { get; set; }

        public long? PhongBenhVienId { get; set; }

        public long NhanVienId { get; set; }

        public string TenKhoaPhong { get; set; }

        public string TenNhanVien { get; set; }

        public bool? LaPhongLamViecChinh { get; set; }
    }

    public class KhoaPhongNhanVienMultiViewModel
    {
        public long KhoaPhongId { get; set; }

        public long[] NhanVienIds { get; set; }
    }
}
