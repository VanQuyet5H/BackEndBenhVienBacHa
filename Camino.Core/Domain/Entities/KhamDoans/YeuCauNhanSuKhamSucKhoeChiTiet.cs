using Camino.Core.Domain.Entities.NhanViens;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class YeuCauNhanSuKhamSucKhoeChiTiet : BaseEntity
    {
        public long YeuCauNhanSuKhamSucKhoeId { get; set; }
        public Enums.NguonNhanSu NguonNhanSu { get; set; }
        public string HoTen { get; set; }
        public long? NhanVienId { get; set; }
        public string DonVi { get; set; }
        public string ViTriLamViec { get; set; }
        public string SoDienThoai { get; set; }
        public int DoiTuongLamViec { get; set; }
        public long? NguoiGioiThieuId { get; set; }
        public long? NhanSuKhamSucKhoeTaiLieuDinhKemId { get; set; }
        public string GhiChu { get; set; }

        public virtual YeuCauNhanSuKhamSucKhoe YeuCauNhanSuKhamSucKhoe { get; set; }
        public virtual NhanVien NhanVien { get; set; }
        public virtual NhanVien NguoiGioiThieu { get; set; }
        public virtual NhanSuKhamSucKhoeTaiLieuDinhKem NhanSuKhamSucKhoeTaiLieuDinhKem { get; set; }
    }
}
