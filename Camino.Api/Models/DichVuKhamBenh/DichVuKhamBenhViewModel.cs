using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DichVuKhamBenh
{
    public class DichVuKhamBenhViewModel : BaseViewModel
    {
        public DichVuKhamBenhViewModel()
        {
            DichVuKhamBenhThongTinGias = new List<DichVuKhamBenhThongTinGiaViewModel>();
        }
        public string MaChung { get; set; }
        public string MaTT37 { get; set; }
        public string TenChung { get; set; }
        public long? KhoaId { get; set; }
        public string TenKhoa { get; set; }
        public HangBenhVien? HangBenhVien { get; set; }
        public string TenHangBenhVien => HangBenhVien.GetDescription();
        public string MoTa { get; set; }
        public List<DichVuKhamBenhThongTinGiaViewModel> DichVuKhamBenhThongTinGias { get; set; }
    }

    public class DichVuKhamBenhThongTinGiaViewModel : BaseViewModel
    {
        public decimal? Gia { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
    }
}
