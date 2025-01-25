using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuGiuongBenhVien
{
    public class NhomGiaDichVuGiuongBenhVienViewModel
    {
        public string Ten { get; set; }
       
        public List<DichVuGiuongBenhVienGiaBenhVienViewModel> DichVuGiuongBenhVienGiaBenhViens{ get; set; }
        
    }
}
