namespace Camino.Api.Models.GoiDichVu
{
    public class GoiDichVuChiTietVatTuViewModel : BaseViewModel
    {
        public long GoiDichVuId { get; set; }
        
        public long VatTuBenhVienId { get; set; }

        public string VatTuBenhVien { get; set; }

        public float SoLuong { get; set; }
    }
}
