using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.KetQuaVaKetLuanMaus
{
    public class KetQuaVaKetLuanMau : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string KetQuaMau { get; set; }
        public string KetLuanMau { get; set; }
        //public string NoiDung { get; set; }
        //public Enums.LoaiKetQuaVaKetLuanMau LoaiKetQuaVaKetLuanMau { get; set; }
    }
}
