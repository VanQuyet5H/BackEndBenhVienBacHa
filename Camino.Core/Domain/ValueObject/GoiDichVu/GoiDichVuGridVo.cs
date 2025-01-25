using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.GoiDichVu
{
    public class GoiDichVuGridVo : GridItem
    {
        public long GoiDichVuUd { get; set; }

        public string LoaiGoi { get; set; }

        public string TenGoi { get; set; }

        public bool CoChietKhau { get; set; }

        public long? ChiPhiGoiDichVu { get; set; }

        public string NgayBatDauDisplay { get; set; }

        public string NgayKetThucDisplay { get; set; }

        public string MoTa { get; set; }

        public string NguoiTaoDisplay { get; set; }

        public bool? IsDisabled { get; set; }

        public string TyLeChietKhau { get; set; }
    }
    public class GoiDichVuType
    {
        public long Id { get; set; }

        public long TypeGoiDichVu { get; set; }
    }
}
