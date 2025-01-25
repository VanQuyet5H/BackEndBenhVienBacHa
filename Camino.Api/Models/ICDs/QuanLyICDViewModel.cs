using Camino.Core.Domain;
namespace Camino.Api.Models.ICDs
{
    public class QuanLyICDViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string TenTiengViet { get; set; }
        public string TenTiengAnh { get; set; }
        public long? LoaiICDId { get; set; }
        public string TenLoaiICD { get; set; }
        public string ChuanDoanLamSan { get; set; }
        public string ThongTinThamKhaoChoBenhNhan { get; set; }
        public string TenGoiKhac { get; set; }
        public bool? HieuLuc { get; set; }
        public string LoiDanCuaBacSi { get; set; }
        public bool? ManTinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public bool? BenhThuongGap { get; set; }
        public bool? BenhNam { get; set; }
        public bool? KhongBaoHiem { get; set; }
        public bool? NgoaiDinhSuat { get; set; }
        public long? KhoaId { get; set; }
        public string TenKhoa { get; set; }
        public string MoTa { get; set; }
        public string MaChiTiet { get; set; }
    }
}
