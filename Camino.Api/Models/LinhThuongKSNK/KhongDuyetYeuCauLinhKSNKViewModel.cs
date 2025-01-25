using Camino.Api.Models.LinhKSNK;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.LinhThuongKSNK
{
 
    public class KhongDuyetYeuCauLinhKSNKViewModel : BaseViewModel
    {
        public KhongDuyetYeuCauLinhKSNKViewModel()
        {
            DuyetYeuCauLinhVatTuChiTiets = new List<DuyetYeuCauLinhKSNKChiTietViewModel>();
        }
        public string LyDoKhongDuyet { get; set; }
        public List<DuyetYeuCauLinhKSNKChiTietViewModel> DuyetYeuCauLinhVatTuChiTiets { get; set; }
        [Timestamp]
        public virtual byte[] LastModified { get; set; }
    }
}
