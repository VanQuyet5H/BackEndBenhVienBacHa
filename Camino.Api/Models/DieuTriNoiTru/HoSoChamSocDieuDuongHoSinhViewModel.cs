using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class HoSoChamSocDieuDuongHoSinhViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public string BacSiKhaiThac { get; set; }
    }
}
