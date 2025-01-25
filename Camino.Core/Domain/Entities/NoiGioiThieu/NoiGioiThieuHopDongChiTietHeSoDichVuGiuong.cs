using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.NoiGioiThieu
{
    public class NoiGioiThieuHopDongChiTietHeSoDichVuGiuong : BaseEntity
    {
        public long NoiGioiThieuHopDongId { get; set; }
        public long DichVuGiuongBenhVienId { get; set; }
        public long NhomGiaDichVuGiuongBenhVienId { get; set; }
        public decimal DonGiaBenhVien { get; set; }
        public decimal DonGiaGioiThieuTuLan1 { get; set; }
        public decimal HeSoGioiThieuTuLan1 { get; set; }
        public decimal? DonGiaGioiThieuTuLan2 { get; set; }
        public decimal? HeSoGioiThieuTuLan2 { get; set; }
        public decimal? DonGiaGioiThieuTuLan3 { get; set; }
        public decimal? HeSoGioiThieuTuLan3 { get; set; }

        public virtual NoiGioiThieuHopDong NoiGioiThieuHopDong { get; set; }
        public virtual DichVuGiuongBenhVien DichVuGiuongBenhVien { get; set; }
        public virtual NhomGiaDichVuGiuongBenhVien NhomGiaDichVuGiuongBenhVien { get; set; }
    }
}
