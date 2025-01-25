using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuGiuongBenhVien
{
    public class DichVuGiuongBenhVienGiaBaoHiemViewModel : BaseViewModel
    {
        public long? DichVuGiuongBenhVienId { get; set; }
        public decimal? Gia { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public DichVuGiuongBenhVienViewModel DichVuGiuongBenhVien { get; set; }
    }
}
