using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;


namespace Camino.Api.Models.DieuTriNoiTru
{
    public class HoSoKhacBanKiemTiemChungTreEmViewModel : BaseViewModel
    {
        public HoSoKhacBanKiemTiemChungTreEmViewModel()
        {
            NoiTruHoSoKhacFileDinhKems = new List<NoiTruHoSoKhacFileDinhKemViewModel>();
            DuocPhamIds = new List<long>();
        }
        public List<long> DuocPhamIds { get; set; }
        public bool? SotHaThanNhiet { get; set; }
        public bool? NgheTimBatThuong { get; set; }
        public bool? NghePhoiBatThuong { get; set; }
        public bool? TriGiacBatThuong { get; set; }
        public bool? CanNangDuoi2000g { get; set; }
        public bool? CoCacChongChiDinhKhac { get; set; }
        public bool? DuDieuKienTiemChung { get; set; }
        public bool? TamHoanTiemChung { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public LoaiHoSoDieuTriNoiTru? LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public string TenNhanVienThucHien { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay { get; set; }
        public List<NoiTruHoSoKhacFileDinhKemViewModel> NoiTruHoSoKhacFileDinhKems { get; set; }
    }
}
