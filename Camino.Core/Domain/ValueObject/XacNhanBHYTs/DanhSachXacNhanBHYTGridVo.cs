using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.XacNhanBHYTs
{
    public class DanhSachXacNhanBhytDaHuongGridVo : DanhSachXacNhanBhytChuaHuongGridVo
    {
    }

    public class DanhSachXacNhanBhytChuaHuongGridVo : GridItem
    {
        public bool CheckedDefault { get; set; }

        // id dành cho đơn thuốc thanh toán
        public long? IdDatabaseDonThuocThanhToan { get; set; }

        public string Nhom => GroupType.GetDescription();

        public Enums.EnumNhomGoiDichVu GroupType { get; set; }

        public string MaDichVu { get; set; }

        public long? DichVuId { get; set; }

        public long? YeuCauKhamBenhId { get; set; }

        public string TenDichVu { get; set; }

        public string NhanVienChiDinh { get; set; }

        public string NoiChiDinh { get; set; }

        public string LoaiGia { get; set; }

        public decimal SoLuong { get; set; }

        public decimal? DonGiaBenhVien { get; set; }

        public decimal? ThanhTienBenhVien => DonGiaBenhVien * SoLuong;

        public decimal? DGBHYTThamKhao { get; set; }

        public decimal? ThanhTienBHYTThamKhao => (decimal)((double)DGBHYTThamKhao.GetValueOrDefault() * (double)SoLuong);

        public bool DuocHuongBaoHiem { get; set; }

        public bool? BaoHiemChiTra { get; set; }
        public bool IsDaXacNhan => BaoHiemChiTra != null;
        public long? YeuCauGoiDichVuId { get; set; }

        public bool HuongBhyt => BaoHiemChiTra != false;

        public string IcdChinh { get; set; }

        public string GhiChuIcdChinh { get; set; }

        public int? MucHuongSystem { get; set; }

        public int? MucHuongDaDuyet { get; set; }

        public int? TiLeDv { get; set; }

        public Enums.LoaiDichVuKyThuat LoaiKt { get; set; }

        public bool IsPttt => LoaiKt == Enums.LoaiDichVuKyThuat.ThuThuatPhauThuat;

        public List<IcdKemTheoVo> IcdKemTheos { get; set; }

        public bool CanModify => TrangThaiThanhToan != Enums.TrangThaiThanhToan.DaThanhToan;

        public bool ShowHistory { get; set; }

        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }

        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThaiKhamBenh { get; set; }

        public Enums.EnumTrangThaiYeuCauDichVuKyThuat TrangThaiDichVuKyThuat { get; set; }
        public Enums.EnumTrangThaiYeuCauTruyenMau TrangThaiYeuCauTruyenMau { get; set; }

        public Enums.EnumYeuCauDuocPhamBenhVien TrangThaiDuocPhamBenhVien { get; set; }

        public Enums.EnumTrangThaiGiuongBenh TrangThaiGiuongBenh { get; set; }

        public Enums.TrangThaiDonThuocThanhToan TrangThaiDonThuocThanhToan { get; set; }

        public Enums.EnumYeuCauVatTuBenhVien TrangThaiVatTuBenhVien { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");
    }

    public class IcdKemTheoVo
    {
        public string Icd { get; set; }

        public string GhiChu { get; set; }
    }

    public class LichSuXacNhanBhytChiTietGridVo : GridItem
    {
        // id dành cho đơn thuốc thanh toán
        public long? IdDatabaseDonThuocThanhToan { get; set; }

        public long IdDatabase { get; set; }

        public long STT { get; set; }

        public string Nhom { get; set; }

        public string MaDichVu { get; set; }

        public string TenDichVu { get; set; }

        public string LoaiGia { get; set; }

        public double SoLuong { get; set; }

        public int TiLeDv { get; set; }

        public int MucHuong { get; set; }

        public double PhanTramCuThe => (double)(TiLeDv * MucHuong) / 100;

        public decimal? DonGiaBenhVien { get; set; }

        public decimal? DGBHYTChiTra => ((decimal)PhanTramCuThe * DGBHYTThamKhao.GetValueOrDefault()) / 100;

        public GiaBhytThamKhaoVo GiaBhytThamKhaoVo { get; set; }

        public decimal? ThanhTienBenhVien => DonGiaBenhVien.GetValueOrDefault() * (decimal)SoLuong;

        public decimal? DGBHYTThamKhao => GiaBhytThamKhaoVo?.Gia * GiaBhytThamKhaoVo?.TiLeThanhToan / 100;

        public decimal? ThanhTienBHYTThamKhao => DGBHYTThamKhao.GetValueOrDefault() * (decimal)SoLuong;

        public decimal? TTBHYTChiTra => DGBHYTChiTra.GetValueOrDefault() * (decimal)SoLuong;

        public decimal? BNThanhToan => ThanhTienBenhVien.GetValueOrDefault() - TTBHYTChiTra.GetValueOrDefault();
    }

    public class GiaBhytThamKhaoVo
    {
        public decimal Gia { get; set; }

        public int TiLeThanhToan { get; set; }
    }

    public class DanhSachLichSuXacNhanGridVo
    {
        public long DuyetBaoHiemId { get; set; }

        public double SoLuong { get; set; }

        public decimal DgBh { get; set; }

        public int TiLeDv { get; set; }

        public int MucHuong { get; set; }
    }

    public class DuyetBaoHiemXacNhanGridVo
    {
        public string NhanVien { get; set; }

        public string NgayDuyet { get; set; }
    }

    public class LichSuVo
    {
        public List<GridLichSu> Value { get; set; }
    }

    public class GridLichSu : GridItem
    {
        public string TenNhanVien { get; set; }

        public string NgayDuyet { get; set; }

        public double SoLuong { get; set; }

        public decimal DgBh { get; set; }

        public decimal TtBh => DgBh * (decimal)SoLuong;

        public int TiLeDv { get; set; }

        public int MucHuong { get; set; }

        public double TiLeBaoHiem => (double)(TiLeDv * MucHuong) / 100;

        public decimal DgBhChiTra => ((decimal)TiLeBaoHiem * DgBh) / 100;

        public decimal TtBhChiTra => DgBhChiTra * (decimal)SoLuong;
    }

    public class LichSuXacNhanVo : GridItem
    {
        public Enums.EnumNhomGoiDichVu Group { get; set; }
    }
}
