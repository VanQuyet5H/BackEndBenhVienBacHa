namespace Camino.Core.Domain.Entities.DichVuBenhVienTongHops
{
    public class DichVuBenhVienTongHop : BaseEntity
    {
        public Enums.EnumDichVuTongHop LoaiDichVuBenhVien { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public long? DichVuGiuongBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }
    }
}