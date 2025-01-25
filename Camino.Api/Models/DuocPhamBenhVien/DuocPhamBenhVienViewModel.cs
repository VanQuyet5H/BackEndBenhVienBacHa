using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DuocPhamBenhVien
{
    public class DuocPhamBenhVienViewModel : BaseViewModel
    {

        //Duoc Pham BV
        public bool HieuLuc { get; set; }
        public string MaDuocPhamBenhVien { get; set; }
        public long? DuocPhamBenhVienPhanNhomConId { get; set; }
        public string TenDuocPhamBenhVienPhanNhomCon { get; set; }
        public long? DuocPhamBenhVienPhanNhomChaId { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string TenDuocPhamBenhVienPhanNhomCha { get; set; }
        public LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public string TenPhanLoaiThuocTheoQuanLy { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");

        //Duoc Pham Quoc Gia
        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string SoDangKy { get; set; }
        public int? STTHoatChat { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public string TenLoaiThuocHoacHoatChat { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public long? DonViTinhId { get; set; }
        public string TenDonViTinh { get; set; }
        public long? DuongDungId { get; set; }
        public string TenDuongDung { get; set; }
        public string HamLuong { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string DangBaoChe { get; set; }
        public string HuongDan { get; set; }
        public string MoTa { get; set; }
        public string ChiDinh { get; set; }
        public string ChongChiDinh { get; set; }
        public string LieuLuongCachDung { get; set; }
        public string TacDungPhu { get; set; }
        public string ChuYDePhong { get; set; }
        public bool? IsDisabled { get; set; }
        public bool? LaThucPhamChucNang { get; set; }
        public string DieuKienBaoHiemThanhToan { get; set; }
        public int? TheTich { get; set; }
        public int? HeSoDinhMucDonViTinh { get; set; }
        public bool? LaThuocHuongThanGayNghien { get; set; }

        public string MaATC { get; set; }

        public bool? DuocPhamCoDau { get; set; }

        public LoaiDieuKienBaoQuanDuocPham? LoaiDieuKienBaoQuanDuocPham { get; set; }
        public string ThongTinDieuKienBaoQuanDuocPham { get; set; }
        public List<long> MayXetNghiemIds { get; set; }
    }
}
