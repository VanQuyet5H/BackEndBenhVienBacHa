using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.VanBangChuyenMon
{
    public class VanBangChuyenMonViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
    }
    public class ViewIds
    {
        public List<long> ids { get; set; }
    }
}
