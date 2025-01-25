namespace Camino.Api.Models.YeuCauTiepNhan
{
    public class DanhSachDaXacNhanBHYTViewModel : BaseViewModel
    {
        public long? IdDatabaseDonThuocThanhToan { get; set; }

        public long IdDatabase { get; set; }

        public long STT { get; set; }

        public string Nhom { get; set; }

        public string MaDichVu { get; set; }

        public string TenDichVu { get; set; }

        public string LoaiGia { get; set; }

        public int SoLuong { get; set; }

        public decimal? DonGiaBenhVien { get; set; }

        public decimal? ThanhTienBenhVien { get; set; }

        public decimal? DGBHYTThamKhao { get; set; }

        public decimal? ThanhTienBHYTThamKhao { get; set; }

        public decimal? DGBHYTChiTra { get; set; }

        public decimal? TTBHYTChiTra { get; set; }

        public decimal? BNThanhToan { get; set; }

        public bool DuocHuongBaoHiem { get; set; }

        public bool? BaoHiemChiTra { get; set; }

        public bool? flagChange { get; set; }
    }
}
