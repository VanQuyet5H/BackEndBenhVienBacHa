using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh : BaseEntity
    {
        public long NoiGioiThieuHopDongId { get; set; }
        public long DichVuKhamBenhBenhVienId { get; set; }
        public long NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public decimal DonGiaBenhVien { get; set; }
        public LoaiHoaHong LoaiHoaHong { get; set; }
        public decimal? SoTienHoaHong { get; set; }
        public decimal? TiLeHoaHong { get; set; }
        public int ApDungTuLan { get; set; }
        public int? ApDungDenLan { get; set; }

        public virtual NoiGioiThieuHopDong NoiGioiThieuHopDong { get; set; }
        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }
        public virtual NhomGiaDichVuKhamBenhBenhVien NhomGiaDichVuKhamBenhBenhVien { get; set; }
    }
}
