using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DuocPhamBenhViens
{
    public class DuocPhamDaHetHanSearchGridVoItem : GridItem
    {
        public long? KhoId { get; set; }
        public string DuocPham { get; set; }
    }
}