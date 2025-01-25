using Camino.Core.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauGiuongBenhViewModel : BaseViewModel
    {
        public long? YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? DichVuGiuongBenhVienId { get; set; }
        public long? NhomGiaDichVuGiuongBenhVienId { get; set; }

        public string Ten { get; set; }
        public string Ma { get; set; }
        public string MaTT37 { get; set; }
        public Enums.EnumLoaiGiuong? LoaiGiuong { get; set; }
        public string MoTa { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal? Gia { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }

        public long? YeuCauGoiDichVuId { get; set; }
        public long? GoiDichVuId { get; set; }

        public int? TiLeChietKhau { get; set; }

        public long? NhanVienChiDinhId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public long? NoiThucHienId { get; set; }
        public string MaGiuong { get; set; }

        public DateTime? ThoiDiemBatDauSuDung { get; set; }
        public DateTime? ThoiDiemKetThucSuDung { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public long? DaThanhToan { get; set; }
        public long? NoiThanhToanId { get; set; }
        public long? NhanVienThanhToanId { get; set; }
        public DateTime? ThoiDiemThanhToan { get; set; }
        public EnumTrangThaiGiuongBenh? TrangThai { get; set; }
        public TrangThaiThanhToan? TrangThaiThanhToan { get; set; }
        public string GhiChu { get; set; }
        public bool TinhPhi { get; set; }
        public long? GiuongBenhId { get; set; }
        [NotMapped]
        public byte[] YeuCauKhamBenhLastModified { get; set; }
    }
}
