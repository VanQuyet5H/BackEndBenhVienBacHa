using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.ChuanDoan
{
    public class ChuanDoanTemplateVo : GridItem
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }

        public string Ma { get; set; }

        public string Ten { get; set; }
    }
}
