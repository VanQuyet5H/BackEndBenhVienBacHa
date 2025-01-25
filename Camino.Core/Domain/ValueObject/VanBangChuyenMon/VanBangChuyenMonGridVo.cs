using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.VanBangChuyenMon
{
    public class VanBangChuyenMonGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
        public string TenTinhTrang { get; set; }
    }
}
