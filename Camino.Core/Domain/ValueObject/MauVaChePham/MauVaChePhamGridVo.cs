using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.MauVaChePham
{
    public class MauVaChePhamGridVo :GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public Enums.PhanLoaiMau PhanLoaiMau { get; set; }
        public long TheTich { get; set; }
        public string TheTichs { get; set; }
        public long GiaTriToiDa { get; set; }
        public string GiaTriToiDas { get; set; }
        public string GhiChu { get; set; }
    }
}
