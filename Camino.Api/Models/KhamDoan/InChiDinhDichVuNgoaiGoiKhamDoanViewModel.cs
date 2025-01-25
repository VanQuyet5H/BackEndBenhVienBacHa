using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamDoan
{
    public class InChiDinhDichVuNgoaiGoiKhamDoanViewModel
    {
        public InChiDinhDichVuNgoaiGoiKhamDoanViewModel(){
            DichVuChiDinhIns = new List<DichVuChiDinhIns>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public List<long> ListDichVuKyThuatIds { get; set; }
        public string Hosting { get; set; }
        public List<DichVuChiDinhIns> DichVuChiDinhIns { get; set; }
    }
    public class DichVuChiDinhIns
    {
        public long NhomChiDinhId { get; set; }
        public long DichVuChiDinhId { get; set; }
    }
}
