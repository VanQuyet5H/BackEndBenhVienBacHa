using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.LinhVatTu;

namespace Camino.Api.Models.LinhThuongVatTu
{
    public class KhongDuyetYeuCauLinhVatTuViewModel : BaseViewModel
    {
        public KhongDuyetYeuCauLinhVatTuViewModel()
        {
            DuyetYeuCauLinhVatTuChiTiets = new List<DuyetYeuCauLinhVatTuChiTietViewModel>();
        }
        public string LyDoKhongDuyet { get; set; }
        public List<DuyetYeuCauLinhVatTuChiTietViewModel> DuyetYeuCauLinhVatTuChiTiets { get; set; }
        [Timestamp]
        public virtual byte[] LastModified { get; set; }

        public bool? LoaiDuocPhamHayVatTu { get; set; }
    }
}
