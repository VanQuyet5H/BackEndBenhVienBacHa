using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.LinhThuongDuocPham
{
    public class KhongDuyetYeuCauLinhViewModel : BaseViewModel
    {
        public KhongDuyetYeuCauLinhViewModel()
        {
            DuyetYeuCauLinhDuocPhamChiTiets = new List<DuyetYeuCauLinhDuocPhamChiTietViewModel>();
        }
        public string LyDoKhongDuyet { get; set; }
        public List<DuyetYeuCauLinhDuocPhamChiTietViewModel> DuyetYeuCauLinhDuocPhamChiTiets { get; set; }
        [Timestamp]
        public virtual byte[] LastModified { get; set; }
    }
}
