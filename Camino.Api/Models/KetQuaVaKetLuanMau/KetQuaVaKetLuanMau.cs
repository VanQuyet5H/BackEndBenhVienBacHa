using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models
{
    public class KetQuaVaKetLuanMauViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string KetQuaMau { get; set; }
        public string KetLuanMau { get; set; }
        //public string NoiDung { get; set; }
        //public Enums.LoaiKetQuaVaKetLuanMau? LoaiKetQuaVaKetLuanMau { get; set; }
    }
}
