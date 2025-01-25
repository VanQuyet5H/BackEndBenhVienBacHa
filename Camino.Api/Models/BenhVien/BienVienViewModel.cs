
using Camino.Core.Domain;

namespace Camino.Api.Models.BenhVien
{
    public class BienVienViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public string DiaChi { get; set; }
        public Enums.HangBenhVien? HangBenhVien { get; set; }
        public Enums.TuyenChuyenMonKyThuat? TuyenChuyenMonKyThuat { get; set; }
        public string SoDienThoaiLanhDao { get; set; }
        public bool? HieuLuc { get; set; }
        public long? LoaiBenhVienId { get; set; }
        public long? DonViHanhChinhId { get; set; }
        public string TenLoaiBenhVien { get; set; }
        public string TenDonViHanhChinh { get; set; }

        public string HangBenhVienDisplay { get; set; }
        public string TuyenChuyenMonKyThuatDisplay { get; set; }

        public virtual LoaiBenhVien.LoaiBenhVienViewModel LoaiBenhVien { get; set; }
        //public virtual CapQuanLyBenhVien.CapQuanLyBenhVienViewModel CapQuanLyBenhVien { get; set; }
        public virtual DonViHanhChinh.DonViHanhChinhViewModel DonViHanhChinh { get; set; }
    }
}
