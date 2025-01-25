using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.BenefitInsurance
{
    public class InsuranceConfirmVo : GridItem
    {
        public bool CheckedDefault { get; set; }

        public string MaSoTheBHYT { get; set; }

        public long? TheBHYTId { get; set; }

        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }

        public long DichVuId { get; set; }

        public double SoLuong { get; set; }

        public decimal? DonGiaBenhVien { get; set; }

        public decimal? ThanhTienBenhVien { get; set; }

        public decimal DGBHYTThamKhao { get; set; }

        public decimal ThanhTienBHYTThamKhao { get; set; }

        public int? TiLeTheoDichVu { get; set; }

        public int? MucHuong { get; set; }

        public decimal DGBHYTChiTra { get; set; }

        public decimal TTBHYTChiTra { get; set; }

        public decimal BNThanhToan { get; set; }

        public Enums.EnumNhomGoiDichVu GroupType { get; set; }

        public bool IsDaXacNhan { get; set; }
    }
}
