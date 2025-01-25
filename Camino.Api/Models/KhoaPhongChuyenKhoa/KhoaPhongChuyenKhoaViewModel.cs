using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhoaPhongChuyenKhoa
{
    public class KhoaPhongChuyenKhoaViewModel : BaseViewModel
    {
        public long KhoaPhongId { get; set; }
        public long KhoaId { get; set; }

        public string TenKhoa { get; set; }
    }
}
