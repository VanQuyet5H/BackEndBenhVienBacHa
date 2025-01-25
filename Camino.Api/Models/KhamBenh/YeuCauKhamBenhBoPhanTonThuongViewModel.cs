using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class YeuCauKhamBenhBoPhanTonThuongViewModel : BaseViewModel
    {
        public long? YeuCauKhamBenhId { get; set; }
        public string MoTa { get; set; }
        public string HinhAnh { get; set; }
    }
}
