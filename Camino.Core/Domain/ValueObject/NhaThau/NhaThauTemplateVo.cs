namespace Camino.Core.Domain.ValueObject.NhaThau
{
    public class NhaThauTemplateVo
    {
        public long KeyId { get; set; }

        public string DisplayName => Ten;

        public string Ten { get; set; }

        public string DiaChi { get; set; }
    }
    public class NhaThauHopDongTemplateVo
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }

        public string Ten { get; set; }

        public string DiaChi { get; set; }
        public string SoHopDong { get; set; }
        public long NhauThauId { get; set; }

    }
}
