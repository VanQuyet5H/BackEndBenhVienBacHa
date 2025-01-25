using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities;

namespace Camino.Core.Domain
{
    public class Template : BaseEntity
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int Version { get; set; }
        public Enums.TemplateType TemplateType { get; set; }
        public int Language { get; set; }
        public string Description { get; set; }
        public bool? IsDisabled { get; set; }

        public static object Parse(string contentTempalte)
        {
            throw new NotImplementedException();
        }
    }
}
