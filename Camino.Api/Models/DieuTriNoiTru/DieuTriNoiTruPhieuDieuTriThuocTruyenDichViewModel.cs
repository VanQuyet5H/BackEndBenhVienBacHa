
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class DieuTriNoiTruPhieuDieuTriThuocTruyenDichViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? KhoId { get; set; }
        public int LaDuocPhamBHYT { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public double? SoLuong { get; set; }
        public int? TocDoTruyen { get; set; }
        public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
        public double? CachGioTruyenDich { get; set; }
        public int? SoLanDungTrongNgay { get; set; }
        public string GhiChu { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public bool IsDelete { get; set; }
        public bool? LaDichTruyen { get; set; }
        public bool LaTuTruc { get; set; }

    }
}
