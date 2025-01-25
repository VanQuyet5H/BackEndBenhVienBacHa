using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhanSanXuatTheoQuocGia
{
    public class NhaSanXuatTheoQuocGiaViewModel : BaseViewModel
    {
        public long NhaSanXuatId { get; set; }
        public long QuocGiaId { get; set; }
        public string DiaChi { get; set; }
        public string TenQuocGia { get; set; }

    }
}
