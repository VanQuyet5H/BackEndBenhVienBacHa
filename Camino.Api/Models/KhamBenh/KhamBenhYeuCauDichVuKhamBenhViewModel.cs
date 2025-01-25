using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauDichVuKhamBenhViewModel : BaseViewModel
    {
        public long? YeuCauTiepNhanId { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public long? YeuCauKhamBenhTruocId { get; set; }
        public string MaDichVu { get; set; }
        public string MaDichVuTt37 { get; set; }
        public string TenDichVu { get; set; }
        public decimal? Gia { get; set; }
        public int? TiLeUuDai { get; set; }
        public long? GoiDichVuId { get; set; }
        public int? TiLeChietKhau { get; set; }
        public string MoTa { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh? TrangThai { get; set; }
        public long? NhanVienChiDinhId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public bool? DaThanhToan { get; set; }
        public long? NoiThanhToanId { get; set; }
        public long? NhanVienThanhToanId { get; set; }
        public DateTime? ThoiDiemThanhToan { get; set; }
        public DateTime? ThoiDiemDangKy { get; set; }
        public long? NoiDangKyId { get; set; }
        public long? BacSiDangKyId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public long? NoiThucHienId { get; set; }
        public long? BacSiThucHienId { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }

        public string BenhSu { get; set; }
        public string ThongTinKhamTheoKhoa { get; set; }
        public long? IcdchinhId { get; set; }
        public string TenBenh { get; set; }
        public long? NoiKetLuanId { get; set; }
        public long? BacSiKetLuanId { get; set; }
        public Enums.EnumKetQuaDieuTri? KetQuaDieuTri { get; set; }
        public Enums.EnumTinhTrangRaVien? TinhTrangRaVien { get; set; }
        public string HuongDieuTri { get; set; }
        public bool? CoKhamChuyenKhoaTiepTheo { get; set; }
        public bool? CoKeToa { get; set; }
        public bool? CoTaiKham { get; set; }
        public DateTime? NgayTaiKham { get; set; }
        public string GhiChuTaiKham { get; set; }
        public bool? QuayLaiYeuCauKhamBenhTruoc { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }

    }
}
