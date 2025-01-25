using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class GoiDichVuNoiThucHienViewModel: BaseViewModel
    {
        public GoiDichVuNoiThucHienViewModel()
        {
            GoiDichVuChiTietNoiThucHiens = new List<GoiDichVuChiTietNoiThucHienViewModel>();
        }
        public List<GoiDichVuChiTietNoiThucHienViewModel> GoiDichVuChiTietNoiThucHiens { get; set; }
        public long Total { get; set; }
    }
}
