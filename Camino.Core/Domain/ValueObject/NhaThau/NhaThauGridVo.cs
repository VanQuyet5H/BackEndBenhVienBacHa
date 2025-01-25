using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.NhaThau
{
    public class NhaThauGridVo : GridItem
    {
        public string Ten { get; set; }

        public string DiaChi { get; set; }

        public string MaSoThue { get; set; }

        public string TaiKhoanNganHang { get; set; }

        public string NguoiDaiDien { get; set; }

        public string NguoiLienHe { get; set; }

        public string SoDienThoaiLienHe { get; set; }

        public string SoDienThoaiDisplay { get; set; }

        public string EmailLienHe { get; set; }
    }
}
