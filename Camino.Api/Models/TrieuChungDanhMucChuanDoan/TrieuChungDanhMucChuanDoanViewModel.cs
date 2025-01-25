using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.TrieuChungDanhMucChuanDoan
{
    public class TrieuChungDanhMucChuanDoanViewModel : BaseViewModel
    {
        public long TrieuChungId { get; set; }
        public long DanhMucChuanDoanId { get; set; }

        public string TenDanhMucChuan { get; set; }
    }
}
