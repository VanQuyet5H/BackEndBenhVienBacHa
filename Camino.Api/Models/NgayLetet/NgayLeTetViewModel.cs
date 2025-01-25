using Camino.Core.Domain;

namespace Camino.Api.Models.NgayLeTet
{
    public class NgayLeTetViewModel : BaseViewModel
    {
        public string Ten { get; set; }

        public int? Ngay { get; set; }

        public int? Thang { get; set; }

        public int? Nam { get; set; }

        public bool LeHangNam { get; set; }

        public string GhiChu { get; set; }

    }
}
