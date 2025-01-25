using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.InputStringStored
{
    public class InputStringStoredGridVo : GridItem
    {
        public Enums.InputStringStoredKey? Key { get; set; }
        public string KeyDescription { get; set; }
        public string Value { get; set; }
    }
}
