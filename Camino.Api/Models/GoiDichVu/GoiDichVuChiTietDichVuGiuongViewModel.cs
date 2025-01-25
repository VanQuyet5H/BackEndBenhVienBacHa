namespace Camino.Api.Models.GoiDichVu
{
    public class GoiDichVuChiTietDichVuGiuongViewModel : BaseViewModel
    {
        public long GoiDichVuId { get; set; }

        public long DichVuGiuongBenhVienId { get; set; }

        public string DichVuGiuongBenhVien { get; set; }

        public long NhomGiaDichVuGiuongBenhVienId { get; set; }

        public string NhomGiaDichVuGiuongBenhVien { get; set; }
    }
}
