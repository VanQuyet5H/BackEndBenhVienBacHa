using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.TiemChungs
{
    public class VacxinTiemChungVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public string DonViTinh { get; set; }
        public string NuocSanXuat { get; set; }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay => SoLuongTon.MathRoundNumber(2).ToString();
        public long KhoId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
    }
}