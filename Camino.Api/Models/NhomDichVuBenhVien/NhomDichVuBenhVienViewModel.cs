namespace Camino.Api.Models.NhomDichVuBenhVien
{
    public class NhomDichVuBenhVienViewModel : BaseViewModel
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string MoTa { get; set; }

        public bool IsDefault { get; set; }

        public long? NhomDichVuBenhVienChaId { get; set; }

        public string NhomDichVuBenhVienCha { get; set; }
    }
}
