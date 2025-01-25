using Camino.Core.Domain.Entities.DichVuKyThuats;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat : BaseEntity
    {
        public long NoiGioiThieuHopDongId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public long NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public decimal DonGiaBenhVien { get; set; }
        public decimal DonGiaGioiThieuTuLan1 { get; set; }
        public decimal HeSoGioiThieuTuLan1 { get; set; }
        public decimal? DonGiaGioiThieuTuLan2 { get; set; }
        public decimal? HeSoGioiThieuTuLan2 { get; set; }
        public decimal? DonGiaGioiThieuTuLan3 { get; set; }
        public decimal? HeSoGioiThieuTuLan3 { get; set; }

        public virtual NoiGioiThieuHopDong NoiGioiThieuHopDong { get; set; }
        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
        public virtual NhomGiaDichVuKyThuatBenhVien NhomGiaDichVuKyThuatBenhVien { get; set; }
    }
}
