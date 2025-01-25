using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DuocPhamBenhVienPhanNhoms
{
    public class DuocPhamBenhVienPhanNhomGridVo : GridItem
    {
        public string Ten { get; set; }

        public long? NhomChaId { get; set; }

        public int CapNhom { get; set; }

        public List<DuocPhamBenhVienPhanNhomGridVo> DuocPhamBenhVienPhanNhomChildren { get; set; }
    }
}
