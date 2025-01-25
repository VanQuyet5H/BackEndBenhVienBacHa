using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.GiayChungNhanPhauThuat
{
    public class GiayChungNhanPhauThuatViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }

        public long? TinhTrangRaVienId { get; set; }
        public string TinhTrangRaVienText { get; set; }
        public long? DichVuPTTTId { get; set; }
    }
    public class TinhTrangRaVienHoSoKhacViewModel : BaseViewModel
    {
        public string TenTinhTrangRaVien { get; set; }
    }
    public class GiayChungNhanPhauThuatQueryInfo
    {
        public long YeuCauTiepNhanId { get; set; }
    }
}
