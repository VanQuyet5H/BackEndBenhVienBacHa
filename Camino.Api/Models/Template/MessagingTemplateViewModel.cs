using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.Template
{
    public class MessagingTemplateViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public Enums.MessagingType MessagingType { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Link { get; set; }
        public Enums.MessagePriority MessagePriority { get; set; }
        public Enums.LanguageType LanguageId { get; set; }
        public string Description { get; set; }
        public bool? IsDisabled { get; set; }
    }
}
