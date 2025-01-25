using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.TiemChungs
{
    public class YeuCauKhamTiemChungVo : GridItem
    {
        public YeuCauKhamTiemChungVo()
        {
            MienGiamChiPhis = new List<TiemChungMienGiamChiPhiVo>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public string DichVuKyThuatBenhVienDisplay { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public decimal? Gia { get; set; }
        public long? NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public EnumDanhMucNhomTheoChiPhi? NhomChiPhi { get; set; }
        public int? SoLan { get; set; }
        public int? TiLeUuDai { get; set; }
        public TrangThaiThanhToan? TrangThaiThanhToan { get; set; }
        public EnumTrangThaiYeuCauDichVuKyThuat? TrangThai { get; set; }
        public long? NhanVienChiDinhId { get; set; }
        public string NhanVienChiDinhDisplay { get; set; }
        public long? NoiChiDinhId { get; set; }
        public string NoiChiDinhDisplay { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public DateTime? ThoiDiemDangKy { get; set; }
        public LoaiDichVuKyThuat? LoaiDichVuKyThuat { get; set; }
        public string MaGiaDichVu { get; set; }
        public string TenGiaDichVu { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public string NhanVienThucHienDisplay { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public bool CoDichVuKhuyenMai { get; set; }
        
        public YeuCauDichVuKyThuatTiemChungVo TiemChung { get; set; }

        //BVHD-3733
        public long? YeuCauGoiDichVuId { get; set; }
        public decimal? DonGiaTruocChietKhau { get; set; }
        public decimal? DonGiaSauChietKhau { get; set; }

        //BVHD-3825
        public decimal? SoTienMienGiam { get; set; }
        public long? YeuCauGoiDichVuKhuyenMaiId { get; set; }
        public List<TiemChungMienGiamChiPhiVo> MienGiamChiPhis { get; set; }
    }

    public class TiemChungMienGiamChiPhiVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiMienGiam? LoaiMienGiam { get; set; }
        public Enums.LoaiChietKhau? LoaiChietKhau { get; set; }
        public decimal? SoTien { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public bool? DaHuy { get; set; }
    }

    public class YeuCauDichVuKyThuatTiemChungVo : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public string TenDuocPham { get; set; }
        public string TenDuocPhamTiengAnh { get; set; }
        public string SoDangKy { get; set; }
        public int? STTHoatChat { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public LoaiThuocHoacHoatChat? LoaiThuocHoacHoatChat { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public long? DuongDungId { get; set; }
        public string HamLuong { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string DangBaoChe { get; set; }
        public long? DonViTinhId { get; set; }
        public string HuongDan { get; set; }
        public string MoTa { get; set; }
        public string ChiDinh { get; set; }
        public string ChongChiDinh { get; set; }
        public string LieuLuongCachDung { get; set; }
        public string TacDungPhu { get; set; }
        public string ChuYdePhong { get; set; }
        public long? HopDongThauDuocPhamId { get; set; }
        public long? NhaThauId { get; set; }
        public string SoHopDongThau { get; set; }
        public string SoQuyetDinhThau { get; set; }
        public EnumLoaiThau? LoaiThau { get; set; }
        public EnumLoaiThuocThau? LoaiThuocThau { get; set; }
        public string NhomThau { get; set; }
        public string GoiThau { get; set; }
        public int? NamThau { get; set; }
        public double SoLuong { get; set; }
        public long? XuatKhoDuocPhamChiTietId { get; set; }
        public ViTriTiem ViTriTiem { get; set; }
        public int MuiSo { get; set; }
        public TrangThaiTiemChung TrangThaiTiemChung { get; set; }
        public long? NhanVienTiemId { get; set; }
        public string NhanVienTiemDisplay { get; set; }
        public DateTime? ThoiDiemTiem { get; set; }
        public string LieuLuong { get; set; }
    }
}