using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class NoiDungMauKhamBenhLookupItemVo : LookupItemTemplateVo
    {
        public string NoiDung { get; set; }
    }

    public class NoiDungMauKhamBenhGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string NoiDung { get; set; }
    }
}
