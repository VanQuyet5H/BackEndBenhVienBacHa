using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuGiuongBenhVien
{
    public class DichVuGiuongBenhVienGiaBenhVienViewModel : BaseViewModel
    {
        public long? DichVuGiuongBenhVienId { get; set; }
        public long? NhomGiaDichVuGiuongBenhVienId { get; set; }
        public string NhomGiaDichVuGiuongBenhVienText { get; set; }
        public decimal? Gia { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public bool? BaoPhong { get; set; }

        public DichVuGiuongBenhVienViewModel DichVuGiuongBenhVien { get; set; }
        public virtual NhomGiaDichVuGiuongBenhVienViewModel NhomGiaDichVuGiuongBenhVien { get; set; }
    }
}
