namespace Camino.Core.Domain.ValueObject.Thuoc
{
    public class DuocPhamTemplateVo
    {
        public long KeyId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }

        public string DisplayName => Ten;

        public string Ten { get; set; }

        public string HoatChat { get; set; }

        public string NhaSanXuat { get; set; }

        public decimal? Gia { get; set; }
        public double? SoLuongChuaNhap { get; set; }

        public bool? SuDungTaiBenhVien { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string MaDuocPhamBenhVien { get; set; }
        public int? Rank { get; set; }

    }
    public class DuocPhamTemplate
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }

        public string Ten { get; set; }

        public string HoatChat { get; set; }

        public string SoDangKy { get; set; }
    }
}
