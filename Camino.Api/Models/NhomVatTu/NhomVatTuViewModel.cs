using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhomVatTu
{
    public class NhomVatTuViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long? NhomVatTuChaId { get; set; }
        public int? CapNhom { get; set; }
    }
}
