namespace Camino.Api.Models.GiuongBenhs
{
    public class GiuongBenhViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public long? KhoaId { get; set; }
        public long? PhongBenhVienId { get; set; }
        public bool? CoHieuLuc { get; set; }
        public bool? LaGiuongNoi { get; set; }
    }
}