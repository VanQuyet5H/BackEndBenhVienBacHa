
namespace Camino.Core.Domain.Entities.InputStringStoreds
{
    public class InputStringStored : BaseEntity
    {
        public Enums.InputStringStoredKey Set { get; set; }
        public string Value { get; set; }
    }
}
