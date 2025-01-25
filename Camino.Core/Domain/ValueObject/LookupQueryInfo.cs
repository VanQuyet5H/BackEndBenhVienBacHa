using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using MessagePack.Formatters;

namespace Camino.Core.Domain.ValueObject
{
    public class LookupQueryInfo
    {
        public LookupQueryInfo()
        {
            // defaults
            Take = 50;
        }
        public string ParameterDependencies { get; set; }
        public int Id { get; set; }
        public string Query { get; set; }
        public int Take { get; set; }
    }
}
