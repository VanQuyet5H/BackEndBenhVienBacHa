using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject
{
    public class NoiDungLoiDanBacSiLookupItemVo : LookupItemVo
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string HuongXuLyLoiDanBacSi { get; set; }
    }
}
