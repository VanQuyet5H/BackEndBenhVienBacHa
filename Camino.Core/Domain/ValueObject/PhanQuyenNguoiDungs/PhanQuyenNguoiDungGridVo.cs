using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.PhanQuyenNguoiDungs
{
    public class PhanQuyenNguoiDungGridVo : GridItem
    {
        public string Ten { get; set; }

        public string LoaiNguoiDung { get; set; }

        public bool Quyen { get; set; }
    }
}
