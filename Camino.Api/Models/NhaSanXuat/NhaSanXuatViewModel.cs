using Camino.Core.Domain.Entities.NhaSanXuatTheoQuocGias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.NhanSanXuatTheoQuocGia;

namespace Camino.Api.Models.NhaSanXuat
{
    public class NhaSanXuatViewModel :BaseViewModel
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string TenQuocGia { get; set; }
        public IList<NhaSanXuatTheoQuocGiaViewModel> NhaSanXuatTheoQuocGias { get; set; }
        

    }
}
