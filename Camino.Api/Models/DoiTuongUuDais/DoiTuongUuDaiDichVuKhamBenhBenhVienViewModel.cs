using Camino.Api.Models.DichVuKhamBenhBenhViens;
using Camino.Api.Models.DichVuKyThuatBenhVien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DoiTuongUuDais
{
    public class DoiTuongUuDaiDichVuKhamBenhBenhVienViewModel:BaseViewModel
    {
        public DoiTuongUuDaiDichVuKhamBenhBenhVienViewModel()
        {
            DichVuKhamBenhBenhVien = new DichVuKhamBenhBenhVienViewModel();
            DoiTuongUuDai = new List<DoiTuongUuDaiViewModel>();
            ListDichVuKhamBenhBenhVienId = new List<long>();
        }
        public long? DoiTuongUuDaiId { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public string DichVuKhamBenhModelText { get; set; }
        public int? TiLeUuDai { get; set; }
        public long? DichVuKhamBenhBenhVienOld { get; set; }
        public List<long> ListDichVuKhamBenhBenhVienId { get; set; }
        public DichVuKhamBenhBenhVienViewModel DichVuKhamBenhBenhVien { get; set; }
        public List<DoiTuongUuDaiViewModel> DoiTuongUuDai { get; set; }
    }
}
