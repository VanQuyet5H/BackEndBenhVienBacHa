using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.TiemChung
{
    public class YeuCauKhamTiemChungViewModel : BaseViewModel
    {
        public YeuCauKhamTiemChungViewModel()
        {
            MienGiamChiPhis = new List<TiemChungMienGiamChiPhiViewModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatKhamSangLocTiemChungId { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public string DichVuKyThuatBenhVienDisplay { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public string NoiThucHienDisplay { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public decimal? Gia { get; set; }
        public decimal ThanhTien => Gia.GetValueOrDefault() * Convert.ToDecimal(TiemChung?.SoLuong ?? 0);
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public EnumDanhMucNhomTheoChiPhi? NhomChiPhi { get; set; }
        public int? SoLan { get; set; }
        public int? TiLeUuDai { get; set; }
        public TrangThaiThanhToan? TrangThaiThanhToan { get; set; }
        public EnumTrangThaiYeuCauDichVuKyThuat? TrangThai { get; set; }
        public string TrangThaiDisplay => TrangThai?.GetDescription();
        public long? NhanVienChiDinhId { get; set; }
        public string NhanVienChiDinhDisplay { get; set; }
        public long? NoiChiDinhId { get; set; }
        public string NoiChiDinhDisplay { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public string ThoiDiemChiDinhDisplay => ThoiDiemChiDinh?.ApplyFormatDateTimeSACH();
        public DateTime? ThoiDiemDangKy { get; set; }
        public string ThoiDiemDangKyDisplay => ThoiDiemDangKy?.ApplyFormatDateTimeSACH();
        public LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public string MaGiaDichVu { get; set; }
        public string TenGiaDichVu { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string NhanVienThucHienDisplay { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public bool CoDichVuKhuyenMai { get; set; }
        public long? NhanVienTiemId { get; set; }
        public string NhanVienTiemDisplay { get; set; }

        public bool? IsVuotQuaSoDuTaiKhoan { get; set; }
        public bool? CoVacxinChuaTiem { get; set; }

        public int? SoThangTuoi { get; set; }

        public bool IsDichVuHuyThanhToan { get; set; }
        public string LyDoHuyDichVu { get; set; }

        public bool? IsKhongTiemChung { get; set; }
        public bool CoThongTinThanhToan => TrangThaiThanhToan == Core.Domain.Enums.TrangThaiThanhToan.BaoLanhThanhToan || TrangThaiThanhToan == Core.Domain.Enums.TrangThaiThanhToan.DaThanhToan || TrangThaiThanhToan == Core.Domain.Enums.TrangThaiThanhToan.CapNhatThanhToan;

        public long? YeuCauGoiDichVuId { get; set; }
        public string Ma4350DichVu { get; set; }
        public decimal? DonGiaTruocChietKhau { get; set; }
        public decimal? DonGiaSauChietKhau { get; set; }

        public TiemChungYeuCauTiepNhanViewModel YeuCauTiepNhan { get; set; }
        public TiemChungYeuCauKhamBenhSangLocViewModel YeuCauKhamBenh { get; set; }

        public TiemChungYeuCauDichVuKyThuatKhamSangLocViewModel KhamSangLocTiemChung { get; set; }
        public TiemChungYeuCauDichVuKyThuatTiemChungViewModel TiemChung { get; set; }
        public bool? IsDaTiem { get; set; }

        //BVHD-3733
        public string TenGoiDichVu { get; set; }

        //BVHD-3825
        public decimal? SoTienMienGiam { get; set; }
        public long? YeuCauGoiDichVuKhuyenMaiId { get; set; }
        public List<TiemChungMienGiamChiPhiViewModel> MienGiamChiPhis { get; set; }
    }

    public class TiemChungMienGiamChiPhiViewModel: BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiMienGiam? LoaiMienGiam { get; set; }
        public Enums.LoaiChietKhau? LoaiChietKhau { get; set; }
        public decimal? SoTien { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public bool? DaHuy { get; set; }
    }
}
