using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.NhaSanXuat
{
    public class NhaSanXuatGridVo : GridItem
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public virtual IList<NhaSanXuatTheoQuocGias.NhaSanXuatTheoQuocGiasGridVo> NhaSanXuatTheoQuocGia { get; set; }
        public string DiaChi { get; set; }
        public long QuocGiaId { get; set; }

    }
}
