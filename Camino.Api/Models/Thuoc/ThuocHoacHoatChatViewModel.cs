namespace Camino.Api.Models.Thuoc
{
    public class ThuocHoacHoatChatViewModel : BaseViewModel
    {
        public long STTHoatChat { get; set; }

        public long STTThuoc { get; set; }

        public string Ma { get; set; }

        public string MaATC { get; set; }

        public string Ten { get; set; }

        public long DuongDungId { get; set; }

        public long NhomThuocId { get; set; }

        public bool? HoiChan { get; set; }

        public long TyLeBaoHiemThanhToan { get; set; }

        public bool? CoDieuKienThanhToan { get; set; }

        public string MoTa { get; set; } // cái này là mô tả của trường có điều kiện thanh toán

        public int? NhomChiPhi { get; set; } // cái này tạm thời chưa cần sử dụng

        public bool? BenhVienHang1 { get; set; }

        public bool? BenhVienHang2 { get; set; }

        public bool? BenhVienHang3 { get; set; }

        public bool? BenhVienHang4 { get; set; }

        public string DuongDung { get; set; }

        public string NhomThuoc { get; set; }
    }
}
