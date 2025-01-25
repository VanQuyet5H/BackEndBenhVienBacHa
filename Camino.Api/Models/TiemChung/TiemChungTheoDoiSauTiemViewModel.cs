using Camino.Core.Helpers;
using System;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.TiemChung
{
    public class TiemChungTheoDoiSauTiemViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? NhanVienTheoDoiSauTiemId { get; set; }
        public string NhanVienTheoDoiSauTiemDisplay { get; set; }
        public DateTime? ThoiDiemTheoDoiSauTiem { get; set; }
        public string GhiChuTheoDoiSauTiem { get; set; }
        public LoaiPhanUngSauTiem? LoaiPhanUngSauTiem { get; set; }
        public string LoaiPhanUngSauTiemDisplay => LoaiPhanUngSauTiem?.GetDescription();
        public string ThongTinPhanUngSauTiem { get; set; }

        public TiemChungThongTinPhanUngSauTiem TiemChungThongTinPhanUngSauTiem { get; set; }
    }
}