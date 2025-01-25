using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.Thuoc
{
    public class NhomThuocGridVo : GridItem
    {
        public string Ten { get; set; }
        public long? NhomChaId { get; set; }
        public int CapNhom { get; set; }
        public string TenLoaiThuocHoacHoatChat { get; set; }
        public Enums.LoaiThuocHoacHoatChat LoaiThuocHoacHoatChat { get; set; }

        public virtual List<NhomThuocGridVo> NhomThuocChildren { get; set; }
    }
}
