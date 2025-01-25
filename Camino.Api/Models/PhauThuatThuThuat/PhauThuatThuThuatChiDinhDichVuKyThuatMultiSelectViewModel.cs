using System.Collections.Generic;

namespace Camino.Api.Models.PhauThuatThuThuat
{
    public class PhauThuatThuThuatChiDinhDichVuKyThuatMultiSelectViewModel
    {
        public PhauThuatThuThuatChiDinhDichVuKyThuatMultiSelectViewModel()
        {
            DichVuKyThuatTuGois = new List<PhauThuatThuThuatChiDinhDichVuTrungTuGoiViewModel>();
        }
        public long? NhomDichVuBenhVienId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long? PhieuDieuTriId { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public List<string> DichVuKyThuatBenhVienChiDinhs { get; set; }
        public List<PhauThuatThuThuatChiDinhDichVuTrungTuGoiViewModel> DichVuKyThuatTuGois { get; set; }
        public string BieuHienLamSang { get; set; }
        public string DichTeSarsCoV2 { get; set; }
    }

    public class PhauThuatThuThuatChiDinhDichVuTrungTuGoiViewModel
    {
        public string Id { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public string TenGoiDichVu { get; set; }
        public long? ChuongTrinhGoiDichVuId { get; set; }
        public long? ChuongTrinhGoiDichVuChiTietId { get; set; }
        public long? DichVuBenhVienId { get; set; }
        public string TenDichVu { get; set; }
    }
}
