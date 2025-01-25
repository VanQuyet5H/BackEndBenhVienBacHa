using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.Template
{
    public class TemplateGridVo : GridItem
    {
        public string Ten { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public int PhienBan { get; set; }
        public Enums.TemplateType LoaiTemplate { get; set; }
        public int NgonNgu { get; set; }
        public string Description { get; set; }
        public DateTime? LastTime { get; set; }

        public string LoaiTemplateText
        {
            get;set;
        }

        public DateTime DateUpdate { get; set; }
        public string DateUpdateText
        {
            get;set;
         //   get { return DateUpdate.ToString("MM/dd/yyyy"); }
        }
      
        public bool? HoatDong { get; set; }
    }
    public class MesagingTemplateGridVo : GridItem
    {
        public string Ten { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public int PhienBan { get; set; }
        public Enums.MessagingType LoaiTemplate { get; set; }
        public Enums.LanguageType NgonNgu { get; set; }
        public string Description { get; set; }

        public string LoaiTemplateText
        {
            get; set;
        }

        public DateTime DateUpdate { get; set; }
        public DateTime? LastTime { get; set; }
        public string DateUpdateText
        {
            get; set;
            //   get { return DateUpdate.ToString("MM/dd/yyyy"); }
        }

        public bool? HoatDong { get; set; }

    }
}
