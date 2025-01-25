using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.NhatKyHeThong
{
    public class NhatKyHeThongViewModel: BaseViewModel
    {
        public Enums.EnumNhatKyHeThong HoatDong { get; set; }
        public string TenHoatDong { get; set; }
        public string MaDoiTuong { get; set; }
        public long? IdDoiTuong { get; set; }
        public string NoiDung { get; set; }
        public string NguoiTao { get; set; }
        public  DateTime? NgayTao { get; set; }
    }
}
