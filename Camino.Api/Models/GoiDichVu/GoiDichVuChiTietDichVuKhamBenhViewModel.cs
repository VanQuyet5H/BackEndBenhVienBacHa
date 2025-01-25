namespace Camino.Api.Models.GoiDichVu
{
    public class GoiDichVuChiTietDichVuKhamBenhViewModel : BaseViewModel
    {
        public long GoiDichVuId { get; set; }

        public long DichVuKhamBenhBenhVienId { get; set; }

        public string DichVuKhamBenhBenhVien { get; set; }

        public long NhomGiaDichVuKhamBenhBenhVienId { get; set; }

        public string NhomGiaDichVuKhamBenhBenhVien { get; set; }
    }
}
