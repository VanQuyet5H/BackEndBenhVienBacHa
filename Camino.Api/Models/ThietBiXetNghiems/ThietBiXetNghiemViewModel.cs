namespace Camino.Api.Models.ThietBiXetNghiems
{
    public class ThietBiXetNghiemViewModel : BaseViewModel
    {
        public long NhomXetNghiemId { get; set; }

        public string NhomXetNghiemDisplay { get; set; }
     
        public long NhomThietBiId { get; set; }

        public string NhomThietBiDisplay { get; set; }

        public string Ma { get; set; }

        public string Ten { get; set; }

        public string Ncc { get; set; }

        public bool HieuLuc { get; set; }

        public bool IsCopy { get; set; }
    }
}
