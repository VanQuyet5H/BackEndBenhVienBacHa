using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DoiTuongUuDais
{
    public class DoiTuongUuDaiViewModel : BaseViewModel
    {
        public DoiTuongUuDaiViewModel()
        {
            DoiTuongUuDaiDichVuKyThuatBenhViens = new List<DoiTuongUuDaiDichVuKyThuatBenhVienViewModel>();
        }
        public string Ten { get; set; }
        public long? DoiTuongId { get; set; }
        public long? DoiTuongOld { get; set; }
        public int? TiLeUuDai { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
        public List<DoiTuongUuDaiDichVuKyThuatBenhVienViewModel> DoiTuongUuDaiDichVuKyThuatBenhViens{ get; set; }
    }
}
