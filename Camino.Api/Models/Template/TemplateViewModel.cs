using Camino.Core.Domain;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.Template
{
    public class TemplateViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int Version { get; set; }
        public Enums.TemplateType TemplateType { get; set; }
        public int Language { get; set; }
        public string Description { get; set; }
        public bool? IsDisabled { get; set; }

        public string LoaiTemplateText
        {
            get { return TemplateType.GetDescription(); }
        }

    }
}
