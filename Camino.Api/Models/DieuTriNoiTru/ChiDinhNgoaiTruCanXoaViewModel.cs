using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class ChiDinhNgoaiTruCanXoaViewModel
    {
        public long DichVuId { get; set; }
        public string LyDoHuyDichVu { get; set; }
    }

    public class GhiNhatThuocVatTuNgoaiTruCanXoaViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public string YeuCauGhiNhanId { get; set; }
    }
}
