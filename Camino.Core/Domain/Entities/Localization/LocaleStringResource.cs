namespace Camino.Core.Domain.Entities.Localization
{
    public  class LocaleStringResource : BaseEntity
    {
        public string ResourceName { get; set; }
        public string ResourceValue { get; set; }
        public int Language { get; set; }
    }
}
