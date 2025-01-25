using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.ChiSoXetNghiems
{
    public class ChiSoXetNghiemGridVo : GridItem
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string ChiSoBinhThuongNam { get; set; }
        public string ChiSoBinhThuongNu { get; set; }
        public int LoaiXetNghiem { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
        public string TenLoaiXetNghiem { get; set; }
        public string TenHieuLuc { get; set; }

    }
}
