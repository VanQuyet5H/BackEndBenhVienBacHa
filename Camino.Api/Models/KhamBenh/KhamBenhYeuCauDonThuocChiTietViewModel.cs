using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.DonViTinh;
using Camino.Api.Models.Thuoc;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauDonThuocChiTietViewModel: BaseViewModel
    {
        public long YeuCauKhamBenhDonThuocId { get; set; }
        public long DuocPhamId { get; set; }
        public bool LaDuocPhamBenhVien { get; set; }
        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string SoDangKy { get; set; }
        public int? StthoatChat { get; set; }
        public int NhomChiPhi { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public int LoaiThuocHoacHoatChat { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public long DuongDungId { get; set; }

        public string TenDuongDung
        {
            get { return DuongDung != null ? DuongDung.Ten : null; }
        }

        public string HamLuong { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string DangBaoChe { get; set; }
        public long DonViTinhId { get; set; }

        public string TenDonViTinh
        {
            get { return DonViTinh != null ? DonViTinh.Ten : null; }
        }

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
        public int? LoaiThau { get; set; }
        public int? LoaiThuocThau { get; set; }
        public string NhomThau { get; set; }
        public string GoiThau { get; set; }
        public int? NamThau { get; set; }
        public decimal? Gia { get; set; }
        public double SoLuong { get; set; }
        public double? SoLuongThanhToan { get; set; }
        public int? SoNgayDung { get; set; }
        public double? DungSang { get; set; }
        public double? DungTrua { get; set; }
        public double? DungChieu { get; set; }
        public double? DungToi { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public bool BenhNhanMuaNgoai { get; set; }
        public bool DaThanhToan { get; set; }
        public bool DaCapThuoc { get; set; }
        public string GhiChu { get; set; }
        public DuocPhamViewModel DuocPham { get; set; }
        public KhamBenhYeuCauDonThuocViewModel YeuCauKhamBenhDonThuoc { get; set; }
        public DuongDungViewModel DuongDung { get; set; }
        public DonViTinhViewModel DonViTinh { get; set; }
    }

    public class KhamBenhYeuCauDonVTYTChiTietViewModel : BaseViewModel
    {
        public long YeuCauKhamBenhDonVTYTId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long NhomVatTuId { get; set; }
        public string DonViTinh { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string MoTa { get; set; }
        public string ChiDinh { get; set; }
        public double SoLuong { get; set; }
        public string GhiChu { get; set; }
    }
}
