using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class TrangThaiThucHienYeuCauDichVuKyThuatViewModel : BaseViewModel
    {
        public long? YeuCauKhamBenhId { get; set; }

        public long? NhanVienThucHienId { get; set; }
        public string TenDichVu { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string LyDoHuyTrangThaiDaThucHien { get; set; }
        public bool LaThucHienDichVu { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
        public bool LaKhamDoanTatCa { get; set; }

        public DateTime? ThoiDiemHoanThanh => ThoiDiemThucHien;
        public long? NhanVienKetLuanId => NhanVienThucHienId;
    }
}
