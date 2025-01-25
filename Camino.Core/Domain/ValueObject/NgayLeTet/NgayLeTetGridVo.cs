using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.NgayLeTet
{
    public class NgayLeTetGridVo : GridItem
    {
        public string Ten { get; set; }

        public int Ngay { get; set; }

        public int Thang { get; set; }

        public int? Nam { get; set; }

        public bool LeHangNam { get; set; }

        public string GhiChu { get; set; }
    }

    public class NgayLeTetSearch
    {
        public string Ten { get; set; }
        public int? Nam { get; set; }
    }

    public class NamSearch
    {
        public string Nam { get; set; }
    }
}
