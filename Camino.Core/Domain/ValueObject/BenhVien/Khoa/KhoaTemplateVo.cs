namespace Camino.Core.Domain.ValueObject.BenhVien.Khoa
{
    public class KhoaTemplateVo
    {
        public long KeyId { get; set; }

        public string DisplayName => Ten;

        public string Ten { get; set; }

        public string Ma { get; set; }
    }
}
