using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.DichVuKyThuat
{
    public class DichVuKyThuatTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string DichVu { get; set; }
        public string Ma { get; set; }
        public string TenKhoa { get; set; }
    }

    public class DichVuKyThuatBenhVienTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string DichVu { get; set; }
        public string Ma { get; set; }
        public string TenKhoa { get; set; }
        public string MaGiaDichVu { get; set; }
        public long NhomDichVuKyThuatId { get; set; }
        public decimal? Gia { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public string TenNhomDichVuBenhVien { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi NhomChiPhi { get; set; }
        public Enums.LoaiDichVuKyThuat LoaiDichVuKyThuat { get; set; }
    }

    public class DichVuKyThuatBenhVienMultiSelectTemplateVo
    {
        public string DisplayName { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public string TenNhomDichVuBenhVien { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenTT43 { get; set; }
        public NoiThucHienUuTienVo NoiThucHien { get; set; }
        public string TenNoiThucHien
        {
            get { return NoiThucHien.DisplayNoiThucHien; }
        }

        public string KeyId
        {
            get
            {
                string templateKeyId = "\"DichVuId\": {0}, \"NhomId\": {1},  \"TenNhom\": \"{2}\", \"NoiThucHienId\": {3}";
                return "{" + string.Format(templateKeyId, DichVuKyThuatBenhVienId, NhomDichVuBenhVienId, TenNhomDichVuBenhVien, NoiThucHien.NoiThucHienId) + "}";
            }
        }
    }

    public class NoiThucHienUuTienVo
    {
        public long NoiThucHienId { get; set; }
        public string DisplayNoiThucHien { get; set; }
    }

    public class ItemChiDinhDichVuKyThuatVo
    {
        public long DichVuId { get; set; }
        public long NhomId { get; set; }
        public string TenNhom { get; set; }
        public long NoiThucHienId { get; set; }
    }

    public class DichVuKyThuatChiDinhDangChonVo
    {
        public DichVuKyThuatChiDinhDangChonVo()
        {
            DichVuChiDinhIds = new List<ItemChiDinhDichVuKyThuatVo>();
        }
        public List<ItemChiDinhDichVuKyThuatVo> DichVuChiDinhIds { get; set; }
    }

    public class DuocPhamVacxinTemplateVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string SoDangKy { get; set; }
        public string MaDuocPhamBenhVien { get; set; }
        public string Ten { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string QuyCach { get; set; }
    }

    public class NoiThucHienDichVuLookupVo
    {
        public long PhongBenhVienId { get; set; }
        public string TenPhongBenhVien { get; set; }
        public long DichVuBenhVienId { get; set; }
    }
}