using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.NgheNghiep
{
    public class NgheNghiepGridVo : GridItem
    {
        public string Ten { get; set; }

        public string TenVietTat { get; set; }

        public string MoTa { get; set; }

        public bool? IsDisabled { get; set; }
    }
}
