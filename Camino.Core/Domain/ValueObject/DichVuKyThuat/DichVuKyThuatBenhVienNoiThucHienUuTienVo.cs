using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DichVuKyThuat
{
    public class DichVuKyThuatBenhVienNoiThucHienUuTienVo
    {
        public long DichVuKyThuatBenhVienId { get; set; }
        public string TenDichVuKyThuatBenhVien { get; set; }
        public string MaDichVuKyThuatBenhVien { get; set; }
        public long PhongBenhVienId { get; set; }
        public string TenPhongBenhVien { get; set; }
        public string MaPhongBenhVien { get; set; }
        public Enums.LoaiNoiThucHienUuTien LoaiNoiThucHienUuTien { get; set; }
    }
}
