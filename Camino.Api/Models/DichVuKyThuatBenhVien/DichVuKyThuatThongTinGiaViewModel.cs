using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuKyThuatBenhVien
{
    public class DichVuKyThuatThongTinGiaViewModel:BaseViewModel
    {
        public Enums.HangBenhVien? HangBenhVien { get; set; }
        public string TenHangBenhVien { get; set; }
        public decimal? Gia { get; set; }
        public string GiaFormat { get; set; }
        public DateTime? TuNgay { get; set; }
        public string TuNgayFormat { get; set; }
        public DateTime? DenNgay { get; set; }
        public string DenNgayFormat { get; set; }
        public string ThongTu { get; set; }
        public string QuyetDinh { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
    }
    public class DichVuKyThuatViewModelError
    {
        public string Ma { get; set; }
        public int TotalThanhCong { get; set; }
        public string Error { get; set; }
    }
}
