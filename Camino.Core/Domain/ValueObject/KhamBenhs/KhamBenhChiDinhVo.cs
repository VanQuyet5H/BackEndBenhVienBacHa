using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class DichVuTheoGoiVo : LookupItemTextVo
    {
        public int NhomDichVu { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
    }
}
