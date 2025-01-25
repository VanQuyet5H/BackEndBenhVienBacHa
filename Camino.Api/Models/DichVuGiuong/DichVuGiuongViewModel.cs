using Camino.Api.Models.Khoa;
using Camino.Core.Domain;
using System;

namespace Camino.Api.Models.DichVuGiuong
{
    public class DichVuGiuongViewModel : BaseViewModel
    {
        public string MaChung { get; set; }
        public string MaTT37 { get; set; }
        public string TenChung { get; set; }
        public long? KhoaId { get; set; }
        public Enums.HangBenhVien? HangBenhVien { get; set; }
        public string MoTa { get; set; }
        //public KhoaViewModel KhoaVM { get; set; }
        public long? Gia { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string TuNgayDisplay { get; set; }
        public string DenNgayDisplay { get; set; }
        public string DVTTGiaMoTa { get; set; }
        public bool? HieuLuc { get; set; }
        public string HieuLucDisplay { get; set; }

    }
}
