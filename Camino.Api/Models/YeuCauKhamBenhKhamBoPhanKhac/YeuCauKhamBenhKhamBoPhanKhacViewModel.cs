using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.YeuCauKhamBenhKhamBoPhanKhac
{
    public class YeuCauKhamBenhKhamBoPhanKhacViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public string NoiDUng { get; set; }
        public long YeuCauKhamBenhId { get; set; }
    }
    public class YeuCauKhamBenhKhamBoPhanKhacReturnViewModel
    {
        public string Ten { get; set; }
        public string NoiDUng { get; set; }
        public long YeuCauKhamBenhId { get; set; }

    }
}
