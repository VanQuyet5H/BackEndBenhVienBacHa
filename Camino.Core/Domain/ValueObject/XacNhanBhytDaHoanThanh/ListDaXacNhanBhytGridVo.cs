using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.XacNhanBhytDaHoanThanh
{
    public class ListDaXacNhanBhytGridVo : GridItem
    {
        // id dành cho đơn thuốc thanh toán
        public long? IdDatabaseDonThuocThanhToan { get; set; }

        public long IdDatabase { get; set; }

        public long Stt { get; set; }

        public string Nhom => GroupType.GetDescription();

        public Enums.EnumNhomGoiDichVu GroupType { get; set; }

        public string MaDichVu { get; set; }

        public string TenDichVu { get; set; }

        public string LoaiGia { get; set; }

        public decimal SoLuong { get; set; }

        public decimal? DonGiaBenhVien { get; set; }

        public decimal? ThanhTienBenhVien => DonGiaBenhVien * SoLuong;

        public decimal? GiaBhyt { get; set; }

        public int? TiLeThanhToanBhyt => 100;

        public decimal? DgbhytThamKhao => GiaBhyt.GetValueOrDefault() * TiLeThanhToanBhyt.GetValueOrDefault() / 100;

        public decimal? ThanhTienBhytThamKhao => (decimal)((double)DgbhytThamKhao.GetValueOrDefault() * (double)SoLuong);

        public int TiLeDv { get; set; }

        public int MucHuong { get; set; }

        public double PhanTramCuThe => (double)(TiLeDv * MucHuong) / 100;

        public decimal? DgbhytChiTra => ((decimal)PhanTramCuThe * DgbhytThamKhao.GetValueOrDefault()) / 100;

        public decimal? TtbhytChiTra => SoLuong * DgbhytChiTra;

        public decimal? BnThanhToan => ThanhTienBenhVien - TtbhytChiTra;

        public bool DuocHuongBaoHiem { get; set; }

        public long? YeuCauGoiDichVuId { get; set; }

        public bool? BaoHiemChiTra { get; set; }

        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThaiKhamBenh { get; set; }

        public Enums.EnumTrangThaiYeuCauDichVuKyThuat TrangThaiDichVuKyThuat { get; set; }

        public Enums.EnumYeuCauDuocPhamBenhVien TrangThaiDuocPhamBenhVien { get; set; }

        public Enums.EnumTrangThaiGiuongBenh TrangThaiGiuongBenh { get; set; }

        public Enums.TrangThaiDonThuocThanhToan TrangThaiDonThuocThanhToan { get; set; }

        public Enums.EnumYeuCauVatTuBenhVien TrangThaiVatTuTieuHao { get; set; }
    }
}
