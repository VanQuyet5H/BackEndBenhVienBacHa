using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DuocPhamBenhViens
{
    public class DuocPhamDaHetHanTemplateGridVo : GridItem
    {
        public string Now { get; set; }

        public List<DuocPhamDaHetHanGridVo> DuocPhamDaHetHans { get; set; }
    }
}
