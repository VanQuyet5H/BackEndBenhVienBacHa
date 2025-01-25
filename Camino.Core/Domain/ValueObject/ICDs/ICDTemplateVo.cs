namespace Camino.Core.Domain.ValueObject.ICDs
{
    public class ICDTemplateVo
    {
        public long KeyId { get; set; }

        public string DisplayName => TenBenh;


        public string TenBenh { get; set; }
        
        public string Ma { get; set; }
    }
}
