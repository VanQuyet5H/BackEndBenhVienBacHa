using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.NhaSanXuatTheoQuocGias
{
    public class NhaSanXuatTheoQuocGiasGridVo :GridItem
    {
        public long NhaSanXuatId { get; set; }
        public long QuocGiaId { get; set; }
        public string Ma { get; set; }
        public string TenQuocGia { get; set; }
    }
}
