using Camino.Core.Domain;

namespace Camino.Api.Models.QuocGia
{
    public class QuocGiaViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public string QuocTich { get; set; }
        public string MaDienThoaiQuocTe { get; set; }
        public string ThuDo { get; set; }
        public Enums.EnumChauLuc ChauLuc { get; set; }
        public string ChauLucText { get; set; }

        public bool? IsDisabled { get; set; }
    }
}
