using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuKhamBenh
{
    public class NhomGiaDichVuKhamBenhBenhVienViewModel:BaseViewModel
    {
        public string Ten { get; set; }
        public List<DichVuKhamBenhBenhVienGiaBenhVienViewModel> DichVuKhamBenhBenhVienGiaBenhViens { get; set; }
    }
}
