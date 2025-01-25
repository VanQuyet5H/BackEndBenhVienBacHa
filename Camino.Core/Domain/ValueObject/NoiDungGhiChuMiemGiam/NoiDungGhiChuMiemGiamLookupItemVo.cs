using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject
{
    public class NoiDungGhiChuMiemGiamLookupItemVo : LookupItemVo
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string NoiDungMiemGiam { get; set; }
    }
}
