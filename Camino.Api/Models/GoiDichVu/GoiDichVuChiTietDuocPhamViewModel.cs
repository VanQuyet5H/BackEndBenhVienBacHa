namespace Camino.Api.Models.GoiDichVu
{
    public class GoiDichVuChiTietDuocPhamViewModel : BaseViewModel
    {
        public long GoiDichVuId { get; set; }

        public long DuocPhamBenhVienId { get; set; }

        public string DuocPhamBenhVien { get; set; }

        public float SoLuong { get; set; }
    }
}
