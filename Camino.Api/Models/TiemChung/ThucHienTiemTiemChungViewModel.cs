using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.TiemChung
{
    public class ThucHienTiemTiemChungViewModel : BaseViewModel
    {
        public Enums.TrangThaiTiemChung? TrangThaiTiemChung { get; set; }
        public long? NhanVienTiemId { get; set; }
        public DateTime? ThoiDiemTiem { get; set; }
    }
}
