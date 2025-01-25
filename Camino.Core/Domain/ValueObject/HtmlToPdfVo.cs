using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject
{
    public class HtmlToPdfVo
    {
        public string UrlApi { get; set; }
        public string MethodApi { get; set; }
        public string Html { get; set; }
        public string FooterHtml { get; set; }
        public string HeaderHtml { get; set; }
        public int? Bottom { get; set; }
        public int? Left { get; set; }
        public int? Right { get; set; }
        public int? Top { get; set; }
        public string PageSize { get; set; } //A0,A1,A2,A3,A4,A5
        public string PageOrientation { get; set; } //Landscape,Portrait
        public int? Zoom { get; set; }
    }
}
