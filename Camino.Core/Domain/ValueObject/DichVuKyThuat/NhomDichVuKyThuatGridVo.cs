using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DichVuKyThuat
{
    public class NhomDichVuKyThuatGridVo : GridItem
    {
        public NhomDichVuKyThuatGridVo()
        {
            NhomDichVuKyThuatChildren = new List<NhomDichVuKyThuatGridVo>();
        }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public int CapNhom { get; set; }
        public long? NhomDichVuKyThuatChaId { get; set; }

        public List<NhomDichVuKyThuatGridVo> NhomDichVuKyThuatChildren { get; set; }

    }
}
