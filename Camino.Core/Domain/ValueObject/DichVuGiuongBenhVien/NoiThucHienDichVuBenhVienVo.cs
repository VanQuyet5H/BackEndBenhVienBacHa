using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien
{
    public class NoiThucHienDichVuBenhVienVo
    {
        public string KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long? KhoaId { get; set; }
        public string KhoaKeyId { get; set; }
        public List<NoiThucHienDichVuBenhVienVo> Items { get; set; }
        public int CountItems { get; set; }
    }

    public class KhoDaChonVo
    {
        public long KhoId { get; set; }
    }

    public class KhoaDaChonVo
    {
        public long KhoaId { get; set; }
    }

    public class ItemNoiThucHienDichVuBenhVienVo
    {
        public long KhoaId { get; set; }
        public long? PhongId { get; set; }
    }

    public class NoiThucHienUuTienDichVuBenhVienVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string TenKhoa { get; set; }
    }

    public class NoiThucHienDichVuBenhVienDichVuBenhVienVo
    {
        public NoiThucHienDichVuBenhVienDichVuBenhVienVo()
        {
            NoiThucHienIds = new List<ItemNoiThucHienDichVuBenhVienVo>();
        }
        public List<ItemNoiThucHienDichVuBenhVienVo> NoiThucHienIds { get; set; }
    }
}
