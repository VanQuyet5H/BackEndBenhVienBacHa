using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.ICDs
{
    public class ChuanDoanViewModel : BaseViewModel
    {
        public long? DanhMucChuanDoanId { get; set; }
        public string DanhMucChuanDoanName { get; set; }
        public string Ma { get; set; }
        public string  TenTiengViet{ get; set; }
        public string TenTiengAnh{ get; set; }
        public List<long> ChuanDoanLienKetICDIds { get; set; }
        public List<ChuanDoanLienKetICDViewModel> ChuanDoanLienKetICDViewModels { get; set; }
}
}
