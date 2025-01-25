using Camino.Api.Models.TrieuChungDanhMucChuanDoan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.TrieuChung
{
    public class TrieuChungViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public string TenCha { get; set; }

        public long? TrieuChungChaId { get; set; }
        public int CapNhom { get; set; }
        public List<long> DanhMucChuanIds { get; set; }
        public List<TrieuChungDanhMucChuanDoanViewModel> TrieuChungDanhMucChuanDoans { get; set; }
    }
}
