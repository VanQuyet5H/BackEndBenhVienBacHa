using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.NhomDichVuBenhVien
{
    public class NhomDichVuThuongDungGridVo : GridItem
    {
        public string TenNhom { get; set; }
        public string MoTa { get; set; }
        public bool? TrangThai { get; set; }
    }
}
