using Camino.Core.Helpers;
using System;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.TiemChung
{
    public class TiemChungYeuCauDichVuKyThuatTiemChungViewModel : BaseViewModel
    {
        //public long YeuCauTiepNhanId { get; set; }
        //public long? YeuCauDichVuKyThuatKhamSangLocTiemChungId { get; set; }
        //public long DichVuKyThuatBenhVienId { get; set; }
        //public string DichVuKyThuatBenhVienDisplay { get; set; }
        //public long NhomDichVuBenhVienId { get; set; }
        //public long KhoId { get; set; }
        
        //public decimal? DonGia { get; set; }
        //public decimal ThanhTien => DonGia.GetValueOrDefault() * Convert.ToDecimal(SoLuong);
        //public long? NoiThucHienId { get; set; }
        //public string NoiThucHienDisplay { get; set; }
        //public long? NhanVienChiDinhId { get; set; }
        //public string NhanVienChiDinhDisplay { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
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
        public ViTriTiem? ViTriTiem { get; set; }
        public string ViTriTiemDisplay => ViTriTiem?.GetDescription();
        public int? MuiSo { get; set; }
        public TrangThaiTiemChung TrangThaiTiemChung { get; set; }
        public string TrangThaiTiemChungDisplay => TrangThaiTiemChung.GetDescription();
        public long? NhanVienTiemId { get; set; }
        public string NhanVienTiemDisplay { get; set; }
        public DateTime? ThoiDiemTiem { get; set; }
        public string ThoiDiemTiemDisplay => ThoiDiemTiem?.ApplyFormatDateTimeSACH();
        public string SoLoVacXinDisplay { get; set; }
        public string LieuLuong { get; set; }

        public bool IsDaTiem => TrangThaiTiemChung == TrangThaiTiemChung.DaTiemChung;
    }
}