using Camino.Api.Models.DichVuKhamBenhBenhViens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuKhamBenh
{
    public class DichVuKhamBenhBenhVienGiaBaoHiemViewModel : BaseViewModel
    {
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public decimal? Gia { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public  DichVuKhamBenhBenhVienViewModel DichVuKhamBenhBenhVien { get; set; }
        public bool? DenNgayRequired { get; set; }
    }
}
