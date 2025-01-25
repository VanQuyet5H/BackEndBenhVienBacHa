namespace Camino.Core.Domain.ValueObject.PhuongPhapVoCams
{
    public class PhuongPhapVoCamTemplateVo
    {
        public long KeyId { get; set; }

        public string DisplayName => Ten;

        public string Ten { get; set; }

        public string Ma { get; set; }
    }
}
