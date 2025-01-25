using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class GiayRaVienViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public LoaiHoSoDieuTriNoiTru? LoaiHoSoDieuTriNoiTru { get; set; }
        public string ChanDoan { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string TenNhanVienThucHien { get; set; }
        public long? TruongKhoaId { get; set; }
        public string TenTruongKhoa { get; set; }
        public long? GiamDocChuyenMonId { get; set; }
        public string TenGiamDocChuyenMon { get; set; }
        public long? NoiThucHienId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay { get; set; }
        public string ThongTinHoSo { get; set; }
        public string PhuongPhapDieuTri { get; set; }
        public string GhiChu { get; set; }
        public long? IdGhiChu { get; set; }

    }
}
