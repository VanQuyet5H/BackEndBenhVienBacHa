using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong : BaseEntity
    {
        public long NoiGioiThieuHopDongId { get; set; }
        public long DichVuGiuongBenhVienId { get; set; }
        public long NhomGiaDichVuGiuongBenhVienId { get; set; }
        public decimal DonGiaBenhVien { get; set; }
        public LoaiHoaHong LoaiHoaHong { get; set; }
        public decimal? SoTienHoaHong { get; set; }
        public decimal? TiLeHoaHong { get; set; }
        public int ApDungTuLan { get; set; }
        public int? ApDungDenLan { get; set; }

        public virtual NoiGioiThieuHopDong NoiGioiThieuHopDong { get; set; }
        public virtual DichVuGiuongBenhVien DichVuGiuongBenhVien { get; set; }
        public virtual NhomGiaDichVuGiuongBenhVien NhomGiaDichVuGiuongBenhVien { get; set; }
    }
}
