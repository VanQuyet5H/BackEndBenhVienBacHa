using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.DonViTinh;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Helpers;

namespace Camino.Api.Models.Thuoc
{
    public class DuocPhamViewModel : BaseViewModel
    {
        public string TenLoaiThuocHoacHoatChat => LoaiThuocHoacHoatChat.GetValueOrDefault().GetDescription();
        public string TenDonViTinh { get; set; }
        public string TenDuongDung { get; set; }

        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string SoDangKy { get; set; }
        public int? STTHoatChat { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public Enums.LoaiThuocHoacHoatChat? LoaiThuocHoacHoatChat { get; set; }
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
        public string ChuYDePhong { get; set; }
        public bool? IsDisabled { get; set; }
        public bool? LaThucPhamChucNang { get; set; }
        public bool? SuDungThuocBenhVien { get; set; }
        public string DieuKienBaoHiemThanhToan { get; set; }
        public string MaDuocPhamBenhVien { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string DuocPhamBenhVienPhanNhomModelText { get; set; }
        public  bool HieuLuc { get; set; }
        public int? TheTich { get; set; }
        public int? HeSoDinhMucDonViTinh { get; set; }
        public virtual DonViTinhViewModel DonViTinh { get; set; }
        public virtual DuocPhamBenhVienModel DuocPhamBenhVienModel { get; set; }
        public bool? LaThuocHuongThanGayNghien { get; set; }

        //BVHD-3454
        public bool? ChuaTaoDuocPhamBenhVien { get; set; }
    }
    public class DuocPhamBenhVienModel : BaseViewModel
    {
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string MaDuocPhamBenhVien { get; set; }

    }
    public class DuocPhamKhamBenhViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public long? DonViTinhId { get; set; }
        public long? DuongDungId { get; set; }
        public string SoDangKy { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public Enums.LoaiThuocHoacHoatChat? LoaiThuocHoacHoatChat { get; set; }
        public long YeuCauKhamBenhId { get; set; }
        public double? SoLuong { get; set; }
        public int? SoNgayDung { get; set; }
        public string SangDisplay { get; set; }
        public string TruaDisplay { get; set; }
        public string ChieuDisplay { get; set; }
        public string ToiDisplay { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public string GhiChu { get; set; }
    }

    public class DanhSachNhapDuocPhamExcelError
    {
        public string Ten { get; set; }
        public int TotalThanhCong { get; set; }
        public string Error { get; set; }
    }
}
