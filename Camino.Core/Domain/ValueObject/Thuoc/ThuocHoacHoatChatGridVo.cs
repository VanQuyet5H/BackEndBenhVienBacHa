using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.Thuoc
{
    public class ThuocHoacHoatChatGridVo : GridItem
    {
        public long STTHoatChat { get; set; }

        public long STTThuoc { get; set; }

        public string Ma { get; set; }

        public string MaATC { get; set; }

        public string Ten { get; set; }

        public long DuongDungId { get; set; }

        public string DuongDung { get; set; }

        public bool? HoiChan { get; set; }

        public long TyLeBaoHiemThanhToan { get; set; }

        public bool? CoDieuKienThanhToan { get; set; }

        public long NhomThuocId { get; set; }

        public string NhomThuoc { get; set; }

        public string MoTa { get; set; }

        public bool? BenhVienHang1 { get; set; }

        public bool? BenhVienHang2 { get; set; }

        public bool? BenhVienHang3 { get; set; }

        public bool? BenhVienHang4 { get; set; }
    }
}
