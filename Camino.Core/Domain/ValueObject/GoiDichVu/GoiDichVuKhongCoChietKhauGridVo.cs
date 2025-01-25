using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.GoiDichVu
{
    public class GoiDichVuKhongCoChietKhauGridVo : GridItem
    {
        public long? IdDatabase { get; set; }
        
        public string Nhom { get; set; }
        
        public string Ma { get; set; }
        
        public string TenDichVu { get; set; }
        
        public string LoaiGia { get; set; }
        
        public long? SoLuong { get; set; }
        
        public decimal? DonGia { get; set; }
        
        public decimal? ThanhTien { get; set; }

        public string HoatChat { get; set; }

        public string NhaSX { get; set; }

        public bool? FlagDelete { get; set; }

        public long? IdDichVuKhac { get; set; }
    }
}
