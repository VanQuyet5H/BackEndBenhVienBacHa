using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.PhauThuatThuThuat
{
    public class EkipGridVo : GridItem
    {
        public Enums.EnumNhomChucDanh NhomChucDanh { get; set; }

        public string NhomChucDanhDisplay => NhomChucDanh.GetDescription();

        public long BacSiId { get; set; }

        public string BacSi { get; set; }

        public Enums.EnumVaiTroBacSi? VaiTroBacSi { get; set; }

        public string VaiTroBacSiDisplay => VaiTroBacSi.GetDescription();

        public Enums.EnumVaiTroDieuDuong? VaiTroDieuDuong { get; set; }

        public string VaiTroDieuDuongDisplay => VaiTroDieuDuong.GetDescription();
    }
}
