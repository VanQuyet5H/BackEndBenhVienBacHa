using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.XacNhanBhytDaHoanThanh
{
    public class DuyetChiTietBaoHiemChiTietHoanThanhGridVo : GridItem
    {
        public long? IdDatabaseDonThuocThanhToan { get; set; }

        public long IdDatabase { get; set; }

        public long Stt { get; set; }

        public string Nhom { get; set; }

        public string MaDichVu { get; set; }

        public string TenDichVu { get; set; }

        public string LoaiGia { get; set; }

        public double SoLuong { get; set; }

        public decimal? DonGiaBenhVien { get; set; }
        public decimal? DgbhytChiTra => (decimal)PhanTramCuThe * DgbhytThamKhao.GetValueOrDefault() / 100;

        public GiaBhytThamKhaoVo GiaBhytThamKhaoVo { get; set; }

        public int TiLeDv { get; set; }

        public int MucHuong { get; set; }

        public double PhanTramCuThe => (double)(TiLeDv * MucHuong) / 100;



        public decimal? ThanhTienBenhVien => DonGiaBenhVien.GetValueOrDefault() * (decimal)SoLuong;

        public decimal? DgbhytThamKhao => GiaBhytThamKhaoVo?.Gia * GiaBhytThamKhaoVo?.TiLeThanhToan / 100;

        public decimal? ThanhTienBhytThamKhao => DgbhytThamKhao.GetValueOrDefault() * (decimal)SoLuong;

        public decimal? TtbhytChiTra => DgbhytChiTra.GetValueOrDefault() * (decimal)SoLuong;

        public decimal? BnThanhToan => ThanhTienBenhVien.GetValueOrDefault() - TtbhytChiTra.GetValueOrDefault();
    }

    public class GiaBhytThamKhaoVo
    {
        public decimal Gia { get; set; }

        public int TiLeThanhToan { get; set; }
    }
}
