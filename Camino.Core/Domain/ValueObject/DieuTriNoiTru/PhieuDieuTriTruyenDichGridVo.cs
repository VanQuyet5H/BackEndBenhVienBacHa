using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class PhieuDieuTriTruyenDichGridVo : GridItem
    {
        public PhieuDieuTriTruyenDichGridVo()
        {
            SoLuongDisplay = new List<double>();
            DonGias = new List<decimal>();
            ThanhTiens = new List<decimal>();
            YeuCauDuocPhamBenhIds = new List<long>();
        }
        public List<long> YeuCauDuocPhamBenhIds { get; set; }
        public int? SoThuTu { get; set; }
        public long? NoiTruChiDinhPhaThuocTiemId { get; set; }
        public long? NoiTruChiDinhPhaThuocTruyenId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public string MaHoatChat { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long? KhoId { get; set; }
        public string TenKho { get; set; }
        public string HoatChat { get; set; }
        public string HamLuong { get; set; }
        public string DVT { get; set; }
        public int? TocDoTruyen { get; set; }
        public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        public string DonViTocDoTruyenDisplay => DonViTocDoTruyen?.GetDescription();
        public int? SoLanDungTrongNgay { get; set; }
        public double SoLuong { get; set; }
        public List<double> SoLuongDisplay { get; set; }
        public List<decimal> DonGias { get; set; }
        public List<decimal> ThanhTiens { get; set; }
        public decimal ThanhTien => ThanhTiens.Sum(x => x);
        public string TuongTacThuoc { get; set; }
        public string DiUngThuoc { get; set; }
        public string GhiChu { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public int NhomId => LaDuocPhamBHYT ? 1 : 0;
        public string Nhom => LaDuocPhamBHYT ? "BHYT" : "Không BHYT";
        public string GioSuDung { get; set; }
        public EnumYeuCauDuocPhamBenhVien TrangThai { get; set; }
        public string TrangThaiDisplay => TrangThai.GetDescription();
        public int? ThoiGianBatDauTruyen { get; set; }
        public double? CachGioTruyenDich { get; set; }
        public bool TinhTrang { get; set; }
        public string TinhTrangDisplay => TinhTrang ? "Đã xuất" : "Chưa xuất";
        public string PhieuLinh { get; set; }
        public string PhieuXuat { get; set; }
        public bool? LaDichTruyen { get; set; }
        public int? TheTich { get; set; }
        public bool CoYeuCauTraDuocPhamTuBenhNhanChiTiet { get; set; }
        public bool LaTuTruc { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public bool? LaThuocHuongThanGayNghien { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public string ThoiDiemChiDinhDisplay => ThoiDiemChiDinh?.ApplyFormatDateTime();
        public string TenNhanVienChiDinh { get; set; }
        public int KhuVuc { get; set; }
        public bool CoThucHienYLenh { get; set; }
        public double? SoLanTrenNgay { get; set; }
        public double? CachGioTiem { get; set; }
        public int? ThoiGianBatDauTiem { get; set; }
        public EnumLoaiKhoDuocPham? LoaiKho => LaTuTruc ? EnumLoaiKhoDuocPham.KhoLe : EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2;
        public string LoaiKhoDisplay => LaTuTruc ? "Kho lẻ" : "Kho tổng";
        public string TenDuongDung { get; set; }
        public LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public int? SoLanTrenMui { get; set; }
        //BVHD-3959
        public int DuongDungNumber => BenhVienHelper.GetSoThuThuocTheoDuongDung(DuongDungId);
        //public bool CheckBox { get; set; }
        public long DuongDungId { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");

        // BVHD-3959 
        public bool ChecBoxItem { get; set; }
        public DateTime? ThoiGianDienBien { get; set; }
        public string ThoiGianDienBienDisplayname => ThoiGianDienBien != null ? ThoiGianDienBien.GetValueOrDefault().ApplyFormatDateTime() : "";
    }

    public class ThuocTruyenDichBenhVienVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public long KhoId { get; set; }
        public int LaDuocPhamBHYT { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public double SoLuong { get; set; }
        public int? TocDoTruyen { get; set; }
        public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
        public double? CachGioTruyenDich { get; set; }
        public int? SoLanDungTrongNgay { get; set; }
        public string GhiChu { get; set; }
        public bool? KhongTinhPhi { get; set; }

    }
    public class ApDungThoiGianDienBienThuocDichTruyenVo
    {
        public ApDungThoiGianDienBienThuocDichTruyenVo()
        {
            DataGridDichVuChons = new List<PhieuDieuTriTruyenDichGridVo>();
        }
        public List<PhieuDieuTriTruyenDichGridVo> DataGridDichVuChons { get; set; }

        public DateTime? ThoiGianDienBien { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long PhieuDieuTriId { get; set; }
    }
}
