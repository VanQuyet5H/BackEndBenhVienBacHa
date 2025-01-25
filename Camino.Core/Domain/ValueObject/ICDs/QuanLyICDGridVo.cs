using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;


namespace Camino.Core.Domain.ValueObject.ICDs
{
    public class QuanLyICDGridVo : GridItem
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
        public string ManTinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay { get; set; }
        public string BenhThuongGap { get; set; }
        public string BenhNam { get; set; }
        public string KhongBaoHiem { get; set; }
        public string NgoaiDinhSuat { get; set; }
        public long? KhoaId { get; set; }
        public string TenKhoa { get; set; }
        public string MoTa { get; set; }
        public string MaChiTiet { get; set; }
    }

    public class QuanLyICDExportExcel
    {
        [Width(20)]
        public string Ma { get; set; }
        [Width(40)]
        public string TenTiengViet { get; set; }
        [Width(40)]
        public string TenTiengAnh { get; set; }
        [Width(40)]
        public string TenLoaiICD { get; set; }
        [Width(40)]
        public string ChuanDoanLamSan { get; set; }
        [Width(40)]
        public string ThongTinThamKhaoChoBenhNhan { get; set; }
        [Width(40)]
        public string TenGoiKhac { get; set; }

        [Width(20)]
        public string HieuLuc { get; set; }

        [Width(40)]
        public string LoiDanCuaBacSi { get; set; }

        [Width(10)]
        public string ManTinh { get; set; }

        [Width(20)]
        public string GioiTinhDisplay { get; set; }

        [Width(10)]
        public string BenhThuongGap { get; set; }

        [Width(10)]
        public string BenhNam { get; set; }

        [Width(10)]
        public string KhongBaoHiem { get; set; }

        [Width(10)]
        public string NgoaiDinhSuat { get; set; }

        [Width(20)]
        public string TenKhoa { get; set; }

        [Width(40)]
        public string MoTa { get; set; }

        [Width(20)]
        public string MaChiTiet { get; set; }
    }

    public class QuanLyICDTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public int? Rank { get; set; }
    }

    public class KhoaQuanLyICDTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
    }
}
