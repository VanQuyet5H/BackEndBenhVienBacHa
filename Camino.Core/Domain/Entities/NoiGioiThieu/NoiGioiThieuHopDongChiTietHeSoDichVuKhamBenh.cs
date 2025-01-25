using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh : BaseEntity
    {
        public long NoiGioiThieuHopDongId { get; set; }
        public long DichVuKhamBenhBenhVienId { get; set; }
        public long NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public decimal DonGiaBenhVien { get; set; }
        public decimal DonGiaGioiThieuTuLan1 { get; set; }
        public decimal HeSoGioiThieuTuLan1 { get; set; }
        public decimal? DonGiaGioiThieuTuLan2 { get; set; }
        public decimal? HeSoGioiThieuTuLan2 { get; set; }
        public decimal? DonGiaGioiThieuTuLan3 { get; set; }
        public decimal? HeSoGioiThieuTuLan3 { get; set; }

        public virtual NoiGioiThieuHopDong NoiGioiThieuHopDong { get; set; }
        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }
        public virtual NhomGiaDichVuKhamBenhBenhVien NhomGiaDichVuKhamBenhBenhVien { get; set; }
    }
}
