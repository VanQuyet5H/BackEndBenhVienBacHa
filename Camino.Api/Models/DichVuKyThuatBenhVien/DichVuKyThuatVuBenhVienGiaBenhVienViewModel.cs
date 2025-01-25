using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuKyThuatBenhVien
{
    public class DichVuKyThuatVuBenhVienGiaBenhVienViewModel : BaseViewModel
    {
        public DichVuKyThuatVuBenhVienGiaBenhVienViewModel()
        {
            NhomGiaDichVuKyThuatBenhVien = new NhomGiaDichVuKyThuatBenhVienViewModel();
            DichVuKyThuatBenhVien = new DichVuKyThuatBenhVienViewModel();
        }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public decimal? Gia { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string NhomGiaDichVuKyThuatBenhVienText { get; set; }
        public NhomGiaDichVuKyThuatBenhVienViewModel NhomGiaDichVuKyThuatBenhVien { get; set; }
        public DichVuKyThuatBenhVienViewModel DichVuKyThuatBenhVien { get; set; }
        public bool? DenNgayRequired { get; set; }
    }
}
