using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.KhamBenh
{
    public class KetLuanKhamBenhViewModel : BaseViewModel
    {
        public KetLuanKhamBenhViewModel()
        {
            YeuCauKhamBenhICDKhacs = new List<YeuCauKhamBenhICDKhacViewModel>();
        }
        public long? YeuCauTiepNhanId { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
        public string LoiDanCuaBacSi { get; set; }
        public long? IcdchinhId { get; set; }
        public bool? CoKhamChuyenKhoaTiepTheo { get; set; }
        public bool? CoKeToa { get; set; }
        public bool? CoTaiKham { get; set; }
        public string TenICDChinh { get; set; }
        public string TenBenh { get; set; }
        public string GhiChu { get; set; }
        public long? NoiKetLuanId { get; set; }
        public long? BacSiKetLuanId { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }
        public string HuongDieuTri { get; set; }
        public DateTime? NgayTaiKham { get; set; }
        public string GhiChuTaiKham { get; set; }
        public bool? QuayLaiYeuCauKhamBenhTruoc { get; set; }
        public bool? CheckValidator { get; set; }

        /// <summary>
        /// Update 02/06/2020
        /// </summary>
        public bool? CoNhapVien { get; set; }
        public long? KhoaPhongNhapVienId { get; set; }
        public string TenKhoaPhongNhapVien { get; set; }
        public string LyDoNhapVien { get; set; }
        public bool? CoChuyenVien { get; set; }
        public long? BenhVienChuyenVienId { get; set; }
        public string TenBenhVienChuyenVien { get; set; }
        public string LyDoChuyenVien { get; set; }
        public bool? CoTuVong { get; set; }
        public string TomTatKetQuaCLS { get; set; }
        public string GhiChuICDChinh { get; set; }
        public string CachGiaiQuyet { get; set; }
        public bool? DieuTriNgoaiTru { get; set; }
        public bool? CoDieuTriNgoaiTru { get; set; }
        public string ChanDoanCuaNoiGioiThieu { get; set; }
        public string TinhTrangBenhNhanChuyenVien { get; set; }
        public LyDoChuyenTuyen? LoaiLyDoChuyenVien { get; set; }
        public DateTime? ThoiDiemChuyenVien { get; set; }
        public string PhuongTienChuyenVien { get; set; }
        public long? NhanVienHoTongChuyenVienId { get; set; }
        public string HoTenNhanVienHoTong { get; set; }
        public bool? KhongKeToa { get; set; }
        public bool IsKhamBenhDangKham { get; set; }
        public bool? CoInKeToa { get; set; }
        public string PhuongPhapTrongDieuTri { get; set; }
        public string KetQuaXetNghiemCLS { get; set; }

        public YeuCauDichVuKyThuatViewModel YeuCauDichVuKyThuat { get; set; }
        public List<YeuCauKhamBenhICDKhacViewModel> YeuCauKhamBenhICDKhacs { get; set; }

    }

    public class YeuCauKhamBenhICDKhacViewModel : BaseViewModel
    {
        public long YeuCauKhamBenhId { get; set; }
        public long? ICDId { get; set; }
        public string GhiChu { get; set; }
        public string TenICD { get; set; }

    }

    public class YeuCauDichVuKyThuatViewModel : BaseViewModel
    {
        public long? YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }
        public LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public string TenDichVuHienThi { get; set; }
        public string MaGiaDichVu { get; set; }
        public EnumDanhMucNhomTheoChiPhi? NhomChiPhi { get; set; }
        public decimal? Gia { get; set; }
        public int? SoLan { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
        public EnumTrangThaiYeuCauDichVuKyThuat? TrangThai { get; set; }
        public TrangThaiThanhToan? TrangThaiThanhToan { get; set; }
        public long? NhanVienChiDinhId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public DateTime? ThoiDiemDangKy { get; set; }
        public string TenNhomDichVu { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public bool? DieuTriNgoaiTru { get; set; }
        public DateTime? ThoiDiemBatDauDieuTri { get; set; }
    }

    public class InPhieuChiDinh
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public List<ListDichVuChiDinh> ListDichVuChiDinh { get; set; }
        public string Hosting { get; set; }
        public string NhanVienChiDinh { get; set; }
        public long InChungChiDinh { get; set; }
        public bool KieuInChung { get; set; }
        public string GhiChuCanLamSang { get; set; }
        public bool IsKhamDoanTatCa { get; set; }
        public bool? InDichVuBacSiChiDinh { get; set; }
    }
}
