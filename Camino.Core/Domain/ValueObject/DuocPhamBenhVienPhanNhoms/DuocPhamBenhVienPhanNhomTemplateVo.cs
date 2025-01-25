using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DuocPhamBenhVienPhanNhoms
{
    public class DuocPhamBenhVienPhanNhomTemplateVo
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }

        public int CapNhom { get; set; }

        public long? NhomChaId { get; set; }

        public bool WillRemove { get; set; }
    }
}
