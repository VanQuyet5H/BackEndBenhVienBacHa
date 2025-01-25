using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.BenhVien.Khoa
{
    public class KhoaGridVo : GridItem
    {
        public string Ten { get; set; }

        public string Ma { get; set; }

        public string MoTa { get; set; }

        public bool? IsDisabled { get; set; }
    }
}
