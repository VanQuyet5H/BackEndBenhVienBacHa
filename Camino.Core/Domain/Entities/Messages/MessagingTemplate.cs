namespace Camino.Core.Domain.Entities.Messages
{
    public class MessagingTemplate : BaseEntity
    {
        public string Name { get; set; }
        public Enums.MessagingType MessagingType { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Link { get; set; }
        public Enums.MessagePriority MessagePriority { get; set; }
        public Enums.LanguageType Language { get; set; }
        public string Description { get; set; }
        public bool? IsDisabled { get; set; }
    }
}
