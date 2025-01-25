namespace Camino.Api.Models.GoiDichVu
{
    public class GoiDichVuChiTietDichVuKyThuatViewModel : BaseViewModel
    {
        public long GoiDichVuId { get; set; }

        public long DichVuKyThuatBenhVienId { get; set; }
            
        public string DichVuKyThuatBenhVien { get; set; }

        public long NhomGiaDichVuKyThuatBenhVienId { get; set; }

        public string NhomGiaDichVuKyThuatBenhVien { get; set; }

        public int SoLan { get; set; }
    }
}
