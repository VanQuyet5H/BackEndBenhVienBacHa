using Camino.Api.Models.DichVuKyThuatBenhVien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DoiTuongUuDais
{
    public class DoiTuongUuDaiDichVuKyThuatAddViewModel: BaseViewModel
    {
        public DoiTuongUuDaiDichVuKyThuatAddViewModel()
        {
            DichVuKyThuatBenhVien = new DichVuKyThuatBenhVienViewModel();
            DoiTuongUuDai = new List<DoiTuongUuDaiViewModel>();
            ListDichVuKyThuatBenhVienId = new List<long>();
        }
        public long? DoiTuongUuDaiId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public string DichVuKyThuatModelText { get; set; }
        public int? TiLeUuDai { get; set; }
        public long? DichVuKyThuatBenhVienOld { get; set; }
        public List<long> ListDichVuKyThuatBenhVienId { get; set; }
        public DichVuKyThuatBenhVienViewModel DichVuKyThuatBenhVien { get; set; }
        public List<DoiTuongUuDaiViewModel> DoiTuongUuDai { get; set; }
    }
}
