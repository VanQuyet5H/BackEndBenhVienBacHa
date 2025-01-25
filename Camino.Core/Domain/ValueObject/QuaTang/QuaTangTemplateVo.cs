namespace Camino.Core.Domain.ValueObject.QuaTang
{
    public class QuaTangTemplateVo
    {
        public string DisplayName => Ten;

        public long KeyId { get; set; }

        public string Ten { get; set; }

        public string Dvt { get; set; }
    }
}
