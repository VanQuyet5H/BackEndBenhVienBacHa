using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuKyThuatBenhVien
{
    public class DichVuKyThuatBenhVienGiaBaoHiemViewModel : BaseViewModel
    {
        public DichVuKyThuatBenhVienGiaBaoHiemViewModel()
        {
            DichVuKyThuatBenhVien = new DichVuKyThuatBenhVienViewModel();
        }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public decimal? Gia { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public DichVuKyThuatBenhVienViewModel DichVuKyThuatBenhVien { get; set; }
        public bool? DenNgayRequired { get; set; }
    }
}
