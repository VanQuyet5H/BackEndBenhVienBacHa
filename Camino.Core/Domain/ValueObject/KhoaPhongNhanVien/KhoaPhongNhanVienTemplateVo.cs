using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhoaPhongNhanVien
{
    public class PhongKhamTemplateVo
    {
        public string DisplayName { get; set; }
        public long KeyId { get; set; }
        public string TenNhanVien { get; set; }
        public string MaPhong { get; set; }
        public string TenPhong { get; set; }
        public long PhongKhamId { get; set; }
        public long NhanVienId { get; set; }
        public string Ma => MaPhong;
        public string Ten => TenPhong;
    }
}
