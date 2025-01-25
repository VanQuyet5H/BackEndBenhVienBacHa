using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuKyThuatBenhVien
{
    public class NhomGiaDichVuKyThuatBenhVienViewModel: BaseViewModel
    {
        public NhomGiaDichVuKyThuatBenhVienViewModel()
        {
            DichVuKyThuatVuBenhVienGiaBenhViens = new List<DichVuKyThuatVuBenhVienGiaBenhVienViewModel>();
        }
        public string Ten { get; set; }
        public List<DichVuKyThuatVuBenhVienGiaBenhVienViewModel> DichVuKyThuatVuBenhVienGiaBenhViens { get; set; }
        
    }
}
