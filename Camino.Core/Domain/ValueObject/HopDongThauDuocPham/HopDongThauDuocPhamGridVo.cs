using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.HopDongThauDuocPham
{
    public class HopDongThauDuocPhamGridVo : GridItem
    {
        public string NhaThau { get; set; }

        public string SoHopDong { get; set; }

        public string SoQuyetDinh { get; set; }

        public string CongBoDisplay { get; set; }

        public string NgayKyDisplay { get; set; }

        public string NgayHieuLucDisplay { get; set; }

        public string NgayHetHanDisplay { get; set; }

        public int LoaiThau { get; set; }

        public int LoaiThuocThau { get; set; }

        public string TenLoaiThau { get; set; }

        public string TenLoaiThuocThau { get; set; }

        public string NhomThau { get; set; }

        public string GoiThau { get; set; }

        public int Nam { get; set; }
    }
}
