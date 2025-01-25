namespace Camino.Core.Domain.Entities.CauHinhs
{
    public class CauHinh : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public Enums.DataType DataType { get; set; }
    }
}
