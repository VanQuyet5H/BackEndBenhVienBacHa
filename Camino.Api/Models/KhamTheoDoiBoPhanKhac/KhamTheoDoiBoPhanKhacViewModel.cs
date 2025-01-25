using Camino.Api.Models.KhamTheoDoi;

namespace Camino.Api.Models.KhamTheoDoiBoPhanKhac
{
    public class KhamTheoDoiBoPhanKhacViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public string NoiDung { get; set; }
        public long KhamTheoDoiId { get; set; }

        public KhamTheoDoiViewModel KhamTheoDoi { get; set; }
    }
}
