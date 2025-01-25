using Camino.Api.Models.KhoaPhong;

namespace Camino.Api.Models.PhongBenhVien
{
    public class PhongBenhVienViewModel : BaseViewModel
    {
        public long KhoaPhongId { get; set; }

        public string Ten { get; set; }

        public string Ma { get; set; }

        public string TenKhoaPhong { get; set; }

        public bool? IsDisabled { get; set; }

        public bool? KieuKham { get; set; }
        public string Tang { get; set; }
        public KhoaPhongViewModel KhoaPhong { get; set; }

    }
}
