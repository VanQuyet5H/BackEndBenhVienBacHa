using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.NoiDungMauLoiDanBacSi
{
    public class NoiDungMauLoiDanBacSi : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string HuongXuLyLoiDanBacSi { get; set; }
        public int? LoaiBenhAn { get; set; }
    }
}
