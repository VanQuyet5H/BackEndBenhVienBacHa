using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauDichVuGiuongBenhVienChiPhi : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long DichVuGiuongBenhVienId { get; set; }
        public long NhomGiaDichVuGiuongBenhVienId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string MaTT37 { get; set; }
        public Enums.EnumLoaiGiuong LoaiGiuong { get; set; }
        public string MoTa { get; set; }
        public decimal Gia { get; set; }
        public int SoNgay { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public decimal? SoTienBenhNhanDaChi { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public long? NhanVienHuyThanhToanId { get; set; }
        public string LyDoHuyThanhToan { get; set; }
        public Enums.DoiTuongSuDung? DoiTuongSuDung { get; set; }
        public string GhiChu { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }

        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual DichVuGiuongBenhVien DichVuGiuongBenhVien { get; set; }
        public virtual NhomGiaDichVuGiuongBenhVien NhomGiaDichVuGiuongBenhVien { get; set; }
        public virtual NhanVien NhanVienDuyetBaoHiem { get; set; }
        public virtual NhanVien NhanVienHuyThanhToan { get; set; }
    }
}

