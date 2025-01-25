using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.LichPhanCongNgoaiTru
{
    public class LichTuanGridVo : GridItem
    {
        public int Value { get; set; }

        public string Name { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}
