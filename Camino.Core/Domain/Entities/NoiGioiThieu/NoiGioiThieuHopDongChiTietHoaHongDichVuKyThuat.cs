using Camino.Core.Domain.Entities.DichVuKyThuats;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat : BaseEntity
    {
        public long NoiGioiThieuHopDongId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public long NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public decimal DonGiaBenhVien { get; set; }
        public LoaiHoaHong LoaiHoaHong { get; set; }
        public decimal? SoTienHoaHong { get; set; }
        public decimal? TiLeHoaHong { get; set; }
        public int ApDungTuLan { get; set; }
        public int? ApDungDenLan { get; set; }

        public virtual NoiGioiThieuHopDong NoiGioiThieuHopDong { get; set; }
        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
        public virtual NhomGiaDichVuKyThuatBenhVien NhomGiaDichVuKyThuatBenhVien { get; set; }
    }
}
