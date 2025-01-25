using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.CongTyUuDais
{
    public class CongTyUuDaiGridVo : GridItem
    {
        public string Ten { get; set; }

        public string MoTa { get; set; }

        public bool? IsDisabled { get; set; }
    }
}
