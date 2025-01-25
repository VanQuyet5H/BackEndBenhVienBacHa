namespace Camino.Core.Domain.ValueObject.ThietBiXetNghiems
{
    public class NhomThietBiTemplateVo
    {
        public string DisplayName => Ten;
        
        public long KeyId { get; set; }
        
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string NhaSanXuat { get; set; }
    }
}
