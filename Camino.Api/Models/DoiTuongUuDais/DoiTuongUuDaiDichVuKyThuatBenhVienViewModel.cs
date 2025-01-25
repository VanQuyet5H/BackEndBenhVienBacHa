using Camino.Api.Models.DichVuKyThuatBenhVien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DoiTuongUuDais
{
    public class DoiTuongUuDaiDichVuKyThuatBenhVienViewModel:BaseViewModel
    {
        public DoiTuongUuDaiDichVuKyThuatBenhVienViewModel()
        {
            DichVuKyThuatBenhVien = new DichVuKyThuatBenhVienViewModel();
            DoiTuongUuDai = new DoiTuongUuDaiViewModel();
        }
        public long? DoiTuongUuDaiId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public int? TiLeUuDai { get; set; }

        public DichVuKyThuatBenhVienViewModel DichVuKyThuatBenhVien { get; set; }
        public  DoiTuongUuDaiViewModel DoiTuongUuDai { get; set; }
    }
}
