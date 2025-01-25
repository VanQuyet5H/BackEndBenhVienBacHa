using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.GoiDvMarketings
{
    public class GoiDvMarketingGridVo : GridItem
    {
        public string TenGoiDv { get; set; }

        public string MoTa { get; set; }

        public bool IsDisabled { get; set; }
    }
}
