using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class YeuCauDuocPhamBenhVienData
    {
        public long? NoiTruChiDinhDuocPhamId { get; set; }
        public long? KhoId { get; set; }
        public string TenKho { get; set; }
        public double SoLuong { get; set; }
        public decimal DonGiaBan { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal GiaBan { get; set; }
        public long? XuatKhoDuocPhamId { get; set; }
        public string PhieuLinh { get; set; }
        public string PhieuXuat { get; set; }
        public bool CoYeuCauTraDuocPhamTuBenhNhanChiTiet { get; set; }

        #region Cập nhật 26/12/2022
        public long? YeuCauDuocPhamBenhVienId { get; set; }
        #endregion

        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }
        public Enums.EnumYeuCauDuocPhamBenhVien TrangThai { get; set; }
    }
    public class PhieuDieuTriThuocGridVo : GridItem
    {
        public PhieuDieuTriThuocGridVo()
        {
            SoLuongDisplay = new List<double>();
            DonGias = new List<decimal>();
            ThanhTiens = new List<decimal>();
            YeuCauDuocPhamBenhIds = new List<long>();
        }
        public List<long> YeuCauDuocPhamBenhIds { get; set; }
        public int? SoThuTu { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public string MaHoatChat { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long? KhoId { get; set; }
        public string TenKho { get; set; }
        public string HoatChat { get; set; }
        public string HamLuong { get; set; }
        public string DVT { get; set; }
        public string DungSang { get; set; }
        public string DungTrua { get; set; }
        public string DungChieu { get; set; }
        public string DungToi { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public string ThoiGianDungSangDisplay => ThoiGianDungSang != null ? "(" + ThoiGianDungSang?.ConvertIntSecondsToTime12h() + ")" : "";
        public string ThoiGianDungTruaDisplay => ThoiGianDungTrua != null ? "(" + ThoiGianDungTrua?.ConvertIntSecondsToTime12h() + ")" : "";
        public string ThoiGianDungChieuDisplay => ThoiGianDungChieu != null ? "(" + ThoiGianDungChieu?.ConvertIntSecondsToTime12h() + ")" : "";
        public string ThoiGianDungToiDisplay => ThoiGianDungToi != null ? "(" + ThoiGianDungToi?.ConvertIntSecondsToTime12h() + ")" : "";
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public int? SoLanDungTrongNgay { get; set; }
        public string TenDuongDung { get; set; }
        public double SoLuong { get; set; }
        public List<double> SoLuongDisplay { get; set; }
        public List<decimal> DonGias { get; set; }
        public List<decimal> ThanhTiens { get; set; }
        public decimal ThanhTien => ThanhTiens.Sum(x => x);
        public string ThuocBHYT { get; set; }
        public string TuongTacThuoc { get; set; }
        public string DiUngThuoc { get; set; }
        public string GhiChu { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public int NhomId => LaDuocPhamBHYT ? 1 : 0;
        public string Nhom => LaDuocPhamBHYT ? "BHYT" : "Không BHYT";
        public bool TinhTrang { get; set; }
        public string TinhTrangDisplay => TinhTrang ? "Đã xuất" : "Chưa xuất";
        public string PhieuLinh { get; set; }
        public string PhieuXuat { get; set; }
        public bool? LaDichTruyen { get; set; }
        public bool CoYeuCauTraDuocPhamTuBenhNhanChiTiet { get; set; }
        public bool LaTuTruc { get; set; }
        public string LoaiKhoDisplay => LaTuTruc ? "Kho lẻ" : "Kho tổng";
        public EnumLoaiKhoDuocPham? LoaiKho => LaTuTruc ? EnumLoaiKhoDuocPham.KhoLe : EnumLoaiKhoDuocPham.KhoTongDuocPhamCap2;
        public bool? KhongTinhPhi { get; set; }
        public bool? LaThuocHuongThanGayNghien { get; set; }
        public int? TocDoTruyen { get; set; }
        public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        public string DonViTocDoTruyenDisplay => DonViTocDoTruyen?.GetDescription();
        public double? CachGioTruyenDich { get; set; }
        public string GioSuDung { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public string TenNhanVienChiDinh { get; set; }
        public string ThoiDiemChiDinhDisplay => ThoiDiemChiDinh?.ApplyFormatDateTime();
        public int? TheTich { get; set; }
        public int KhuVuc { get; set; }
        public long DuongDungId { get; set; }
        public LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public int DuongDungNumber
        {
            get
            {
                //BVHD-3959
                return BenhVienHelper.GetSoThuThuocTheoDuongDung(DuongDungId);
                //var numberSort = 0;
                //if (LoaiThuocTheoQuanLy != null && LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)
                //{
                //    numberSort = 1;
                //}
                //else
                //{
                //    numberSort = DuongDungId == 12 ? 2 : (DuongDungId == 1 ? 3 : (DuongDungId == 26 ? 4 : (DuongDungId == 22 ? 5 : 6)));
                //}
                //return numberSort;
            }
        }
        public int? SoLanTrenVien { get; set; }
        public double? CachGioDungThuoc { get; set; }
        public double? LieuDungTrenNgay { get; set; }
        public string LieuDungTrenNgayDisplay => LieuDungTrenNgay.FloatToStringFraction();
        public bool CoThucHienYLenh { get; set; }
        public long? DuoCPhamBenhVienPhanNhomId { get; set; }
        public EnumYeuCauDuocPhamBenhVien TrangThai { get; set; }
        public string TrangThaiDisplay => TrangThai.GetDescription();
        public double? SoLanTrenNgay { get; set; }
        //public bool CheckBox { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");

        // BVHD-3959 
        public bool ChecBoxItem { get; set; }
        public DateTime? ThoiGianDienBien { get; set; }
        public string ThoiGianDienBienDisplayname => ThoiGianDienBien != null ? ThoiGianDienBien.GetValueOrDefault().ApplyFormatDateTime() : "";
    }
    public class ThongTinThuocDieuTriVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long KhoId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public int LoaiDuocPham { get; set; }
        public long NoiTruPhieuDieuTriId { get; set; }
        public int KhuVuc { get; set; }

    }

    public class KhoLookupItemVo
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public EnumLoaiKhoDuocPham LoaiKho { get; set; }
    }

    public class SapXepThuoc
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? PhieuDieuTriHienTaiId { get; set; }
        public bool? LaDichTruyen { get; set; }


    }
    public class ThuocBenhVienVo : GridItem
    {
        public long KhoId { get; set; }
        public int LaDuocPhamBHYT { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public double SoLuong { get; set; }
        public int? SoLanDungTrongNgay { get; set; }
        public double? DungSang { get; set; }
        public double? DungTrua { get; set; }
        public double? DungChieu { get; set; }
        public double? DungToi { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public string GhiChu { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long? PhieuDieuTriHienTaiId { get; set; }
        public bool? LaDichTruyen { get; set; }
        public int? TheTich { get; set; }
        public int? TocDoTruyen { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
        public double? CachGioTruyenDich { get; set; }
        public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public EnumLoaiKhoDuocPham? LoaiKho { get; set; }
        public int? SoLanTrenVien { get; set; }
        public double? CachGioDungThuoc { get; set; }
        public double? LieuDungTrenNgay { get; set; }
        public int? SoLanTrenMui { get; set; }
        public double? CachGioTiem { get; set; }
        public int? SoThuTu { get; set; }
    }

    public class ThuocBenhVienTangGiamSTTVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public long? PhieuDieuTriHienTaiId { get; set; }
        public bool? LaDichTruyen { get; set; }
        public bool? LaTangSTT { get; set; }

    }

    public class InPhieuCongKhaiThuocVatTuReOrder
    {
        public InPhieuCongKhaiThuocVatTuReOrder()
        {
            //Ids = new List<long>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long NoiTruPhieuDieuTriId { get; set; }
        public int LoaiThuocVatTu { get; set; }
        public string HostingName { get; set; }
        //public List<DonThuocChiTietGridVoItem> ListGridThuoc { get; set; }
        //public List<long> Ids { get; set; }

    }

    public class InPhieuThucHienThuocVatTu
    {
        public InPhieuThucHienThuocVatTu()
        {
            //ListGridThuoc = new List<DonThuocChiTietGridVoItem>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long NoiTruPhieuDieuTriId { get; set; }
        public int LoaiThuocVatTu { get; set; }
        public string HostingName { get; set; }
        //public List<DonThuocChiTietGridVoItem> ListGridThuoc { get; set; }
    }
    public class ThongTinChungBenhNhanPhieuDieuTri
    {
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string SoBA { get; set; }
        public string KhoaPhong { get; set; }
        public string HoTenNguoiBenh { get; set; }
        public int? Tuoi { get; set; }
        public string Gioi { get; set; }
        public string SoGiuong { get; set; }
        public string NgayVaoVien { get; set; }
        public string NgayRaVien { get; set; }
        public bool? CoBHYT { get; set; }
        public string MaTheBHYT { get; set; }
        public string Buong { get; set; }
        public string TuoiDisplay { get; set; }

    }

    public class ThongTinThuocVatTuPhieuDieuTri
    {
        public int? STT { get; set; }
        public string TenThuoc { get; set; }
        public string TenVatTu { get; set; }
        public string DonVi { get; set; }
        public string NgayThangNam { get; set; }
        public double SoLuong { get; set; }
        public double TongSo { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string GhiChu { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public int LaDuocPhamBHYTNumber => LaDuocPhamBHYT ? 1 : 2;
        public long DuongDungId { get; set; }
        public LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public int DuongDungNumber
        {
            get
            {
                //BVHD-3959
                return BenhVienHelper.GetSoThuThuocTheoDuongDung(DuongDungId);
                //var numberSort = 0;                
                //numberSort = DuongDungId == 12 ? 2 : (DuongDungId == 1 ? 3 : (DuongDungId == 26 ? 4 : (DuongDungId == 22 ? 5 : 6)));                
                //return numberSort;
            }
        }
        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
    }

    public class ThongTinThuocVatTuPhieuDieuTriThucHien
    {
        public ThongTinThuocVatTuPhieuDieuTriThucHien()
        {
            BSThucHienChiTietSangs = new List<string>();
            BSThucHienChiTietTruas = new List<string>();
            BSThucHienChiTietChieus = new List<string>();
            BSThucHienChiTietTois = new List<string>();

            ChiTietXuats = new List<ChiTietDuocPhamTheoXuatVo>();
        }
        public int? STT { get; set; }
        public bool? LaDichTruyen { get; set; }
        public string TenThuoc { get; set; }
        public string TenVatTu { get; set; }
        public string DuongDung { get; set; }
        public string DonVi { get; set; }
        public double? SoLuong { get; set; }
        public double? TongSo { get; set; }
        public string BSChiDinh { get; set; }
        public string BSChiDinhs { get; set; }
        public string DungSang { get; set; }
        public string DungTrua { get; set; }
        public string DungChieu { get; set; }
        public string DungToi { get; set; }
        public double? SLDungSang { get; set; }
        public double? SLDungTrua { get; set; }
        public double? SLDungChieu { get; set; }
        public double? SLDungToi { get; set; }
        //EnumThoiGianThucHien
        public DateTime? ThoiDiemXacNhanThucHien { get; set; }
        public string DungSangs { get; set; }
        public string DungTruas { get; set; }
        public string DungChieus { get; set; }
        public string DungTois { get; set; }
        public string GhiChu { get; set; }
        public string GhiChus { get; set; }

        public string CachDung
        {
            get
            {
                var cachDung = string.Empty;

                var cd = (!string.IsNullOrEmpty(DungSangs) ? "Sáng " + DungSangs + ", " : "") + (!string.IsNullOrEmpty(DungTruas) ? "Trưa " + DungTruas + ", " : "") +
                        (!string.IsNullOrEmpty(DungChieus) ? "Chiều " + DungChieus + ", " : "") + (!string.IsNullOrEmpty(DungTois) ? "Tối " + DungTois + "," : "");
                cachDung = "<i>" + (DuongDung ?? " ") + " " + (!string.IsNullOrEmpty(cd) ? cd.Trim().Remove(cd.Trim().Length - 1) : "") + " " + GhiChus + "</i>";
                return cachDung;
            }
        }
        public List<string> BSThucHienChiTietSangs { get; set; }
        public List<string> BSThucHienChiTietTruas { get; set; }
        public List<string> BSThucHienChiTietChieus { get; set; }
        public List<string> BSThucHienChiTietTois { get; set; }

        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public long DuongDungId { get; set; }
        public LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public int DuongDungNumber
        {
            get
            {
                //BVHD-3959
                return BenhVienHelper.GetSoThuThuocTheoDuongDung(DuongDungId);
                //var numberSort = 0;                
                //numberSort = DuongDungId == 12 ? 1 : (DuongDungId == 1 ? 2 : (DuongDungId == 26 ? 3 : (DuongDungId == 22 ? 4 : 5)));
                //return numberSort;
            }
        }

        //BVHD-3859
        public long NoiTruChiDinhDuocPhamId { get; set; }

        //BVHD-3859: cập nhật ngày 18/03/2022: gập dược phẩm lại 1 dòng theo tên
        //public DateTime? HanSuDung { get; set; }
        //public string HanSuDungDisplay => HanSuDung?.ApplyFormatDate();
        //public string SoLo { get; set; }
        //public string CachDungDisplay { get; set; }

        public List<string> HanSuDungs =>
            ChiTietXuats.Where(x => x.XuatKhoDuocPhamChiTietId != null)
                .SelectMany(x => x.ChiTietNhaps)
                .Select(x => x.HanSuDungDisplay)
                .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();
        public List<string> SoLos =>
                ChiTietXuats.Where(x => x.XuatKhoDuocPhamChiTietId != null)
                    .SelectMany(x => x.ChiTietNhaps)
                    .Select(x => x.SoLo)
                    .Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

        public string HanSuDungDisplay => HanSuDungs.Any() ? string.Join("; ", HanSuDungs.Where(x => !string.IsNullOrEmpty(x)).Distinct()) : string.Empty;
        //ChiTietXuats.Any(x => x.XuatKhoDuocPhamChiTietId != null && x.ChiTietNhaps.Any(a => a.HanSuDung != null))
        //    ? string.Join("; ",
        //        ChiTietXuats.Where(x => x.XuatKhoDuocPhamChiTietId != null && x.ChiTietNhaps.Any(a => a.HanSuDung != null))
        //            .SelectMany(x => x.ChiTietNhaps)
        //            .Where(x => x.HanSuDung != null).Select(x => x.HanSuDungDisplay).Distinct())
        //    : string.Empty;
        public string SoLo => SoLos.Any() ? string.Join("; ", SoLos.Where(x => !string.IsNullOrEmpty(x)).Distinct()) : string.Empty;
        //public string SoLo =>
        //    ChiTietXuats.Any(x => x.XuatKhoDuocPhamChiTietId != null && x.ChiTietNhaps.Any(a => !string.IsNullOrEmpty(SoLo)))
        //        ? string.Join("; ",
        //            ChiTietXuats.Where(x => x.XuatKhoDuocPhamChiTietId != null && x.ChiTietNhaps.Any())
        //                .SelectMany(x => x.ChiTietNhaps)
        //                .Where(x => !string.IsNullOrEmpty(SoLo)).Select(x => x.SoLo).Distinct())
        //        : string.Empty;
        public string CachDungDisplay { get; set; }

        public List<ChiTietDuocPhamTheoXuatVo> ChiTietXuats { get; set; }

        #region clone
        public ThongTinThuocVatTuPhieuDieuTriThucHien Clone()
        {
            return (ThongTinThuocVatTuPhieuDieuTriThucHien)this.MemberwiseClone();
        }
        #endregion
    }

    #region BVHD-3859
    public class ChiTietDuocPhamTheoXuatVo
    {
        public ChiTietDuocPhamTheoXuatVo()
        {
            ChiTietNhaps = new List<ChiTietNhapKhoDuocPhamVo>();
        }
        public long? XuatKhoDuocPhamChiTietId { get; set; }

        public List<ChiTietNhapKhoDuocPhamVo> ChiTietNhaps { get; set; }
    }

    public class ChiTietNhapKhoDuocPhamVo
    {
        public long XuatKhoDuocPhamChiTietId { get; set; }
        public long NhapKhoDuocPhamChiTietId { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDate();
        public string SoLo { get; set; }

        // là số lượng theo xuất vị trí
        public double SoLuong { get; set; }
    }
    

    #endregion

    public class BSThucHienChiTiet
    {
        public string Ten { get; set; }
        public int Loai { get; set; }
    }


    public class DataThuocVatTuPhieuDieuTri
    {

        public string LogoUrl { get; set; }
        public string Thuoc { get; set; }
        public string VatTu { get; set; }
        public string SoVaoVien { get; set; }
        public string KhoaPhong { get; set; }
        public string HoTenNguoiBenh { get; set; }
        public int? Tuoi { get; set; }
        public string TuoiStr { get; set; }
        public string Gioi { get; set; }
        public string SoGiuong { get; set; }
        public string Buong { get; set; }
        public string ChanDoan { get; set; }
        public string NgayVaoVien { get; set; }
        public string NgayRaVien { get; set; }
        public string NgayThangNam { get; set; }
        public string SoBA { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string MaTheBHYT { get; set; }
        public string NgayDieuTri { get; set; }

    }

    public class YeuCauTraDuocPhamTuBenhNhanChiTietVo : GridItem
    {
        public YeuCauTraDuocPhamTuBenhNhanChiTietVo()
        {
            YeuCauDuocPhamBenhViens = new List<YcDuocPhamBvVo>();
        }
        //public long YeuCauDuocPhamBenhVienId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        //public long KhoTraId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        //public bool TraVeTuTruc { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public List<YcDuocPhamBvVo> YeuCauDuocPhamBenhViens { get; set; }
        public bool LaDichTruyen { get; set; }
    }
    public class YcDuocPhamBvVo
    {
        public long YeuCauDuocPhamBenhVienId { get; set; }
        public double? SoLuongTra { get; set; }
        public double SoLuong { get; set; }
        public double SoLuongDaTra { get; set; }

    }

    public class HoanTraThuocVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long NoiTruChiDinhDuocPhamId { get; set; }
        public long KhoId { get; set; }
        public bool? LaDichTruyen { get; set; }
        public EnumLoaiKhoDuocPham LoaiKho { get; set; }

    }
    public class ThongTinHoanTraThuocVo : GridItem
    {
        public ThongTinHoanTraThuocVo()
        {
            YeuCauDuocPhamBenhViens = new List<ThongTinHoanTraThuocChiTietVo>();
        }
        public long DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public string TenKho { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public string TenNhanVienYeuCau { get; set; }
        public bool? LaDichTruyen { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        //public bool CoYeuCauTraDuocPhamTuBenhNhanChiTiet { get; set; }

        public List<ThongTinHoanTraThuocChiTietVo> YeuCauDuocPhamBenhViens { get; set; }
    }

    public class ThongTinHoanTraThuocChiTietVo
    {
        public bool? KhongTinhPhi { get; set; }
        public long YeuCauDuocPhamBenhVienId { get; set; }
        public double SoLuong { get; set; }
        public string SoLuongDisplay => SoLuong.MathRoundNumber(2).ToString();
        public decimal DonGiaNhap { get; set; }
        public decimal DonGia { get; set; }// => CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public decimal ThanhTien => KhongTinhPhi != true ? DonGia * (decimal)SoLuong : 0;
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public double SoLuongDaTra { get; set; }
        public double? SoLuongTra { get; set; }
    }

    public class YeuCauTraDuocPhamTuBenhNhanChiTietGridVo : GridItem
    {
        public DateTime NgayTra { get; set; }
        public string NgayTraDisplay => NgayTra.ApplyFormatDate();
        public string SoLuongTra { get; set; }
        public string NhanVienTra { get; set; }
        public string SoPhieu { get; set; }
        public bool? DuocDuyet { get; set; }
        public string TinhTrang => DuocDuyet == null ? "Chờ duyệt" : (DuocDuyet == true ? "Đã duyệt" : "Từ chối");
        public DateTime? NgayTao { get; set; }
        public string NgayTaoDisplay => NgayTao?.ApplyFormatDateTimeSACH();

    }

    public class HoanTraVTGridVo
    {
        public string Ids { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public bool LaTuTruc { get; set; }
    }

    public class YeuCauTraVatTuTuBenhNhanChiTietVo : GridItem
    {
        public YeuCauTraVatTuTuBenhNhanChiTietVo()
        {
            YeuCauVatTuBenhViens = new List<YcVTBvVo>();
        }
        public long VatTuBenhVienId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public double? SoLuongTra { get; set; }
        public double SoLuong { get; set; }
        public double SoLuongDaTra { get; set; }
        public List<YcVTBvVo> YeuCauVatTuBenhViens { get; set; }
    }

    public class YcVTBvVo
    {
        public long YeuCauVatTuBenhVienId { get; set; }
        public double? SoLuongTra { get; set; }
        public double SoLuong { get; set; }
        public double SoLuongDaTra { get; set; }

    }


    public class DichVuKyThuatDaThemLookupItem
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
    }

    public class PhieuDieuTriThuocNgoaiBenhVienGridVo : GridItem
    {
        public int STT { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string HamLuong { get; set; }
        public string DVT { get; set; }
        public string DungSang { get; set; }
        public string DungTrua { get; set; }
        public string DungChieu { get; set; }
        public string DungToi { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public string ThoiGianDungSangDisplay { get; set; }
        public string ThoiGianDungTruaDisplay { get; set; }
        public string ThoiGianDungChieuDisplay { get; set; }
        public string ThoiGianDungToiDisplay { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public int? SoLanDungTrongNgay { get; set; }
        public string TenDuongDung { get; set; }
        public double SoLuong { get; set; }
        public string SoLuongDisplay => SoLuong.ApplyNumber();
        public string TuongTacThuoc { get; set; }
        public string DiUngThuoc { get; set; }
        public string GhiChu { get; set; }
        public bool? LaDichTruyen { get; set; }
        public int? TocDoTruyen { get; set; }
        public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        public string DonViTocDoTruyenDisplay => DonViTocDoTruyen?.GetDescription();
        public double? CachGioTruyenDich { get; set; }
        public string GioSuDung { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public string TenNhanVienChiDinh { get; set; }
        public string ThoiDiemChiDinhDisplay => ThoiDiemChiDinh?.ApplyFormatDateTime();
        public int? TheTich { get; set; }
        public int KhuVuc { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");
    }

    public class CapNhatKhongTinhPhi : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public bool KhongTinhPhi { get; set; }
        public bool LaThuoc { get; set; }
    }

    public class DonThuocThanhToanChiTietData
    {
        public long Id { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal DonGiaBan { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
    }

    public class NoiTruDonThuocChiTietGridVoItem : GridItem
    {
        public NoiTruDonThuocChiTietGridVoItem()
        {
            DonThuocThanhToanChiTietIds = new List<long>();
        }

        public List<long> DonThuocThanhToanChiTietIds { get; set; }
        public int? STT { get; set; }
        public long DuocPhamId { get; set; }
        public long NoiTruDonThuocId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public string MaHoatChat { get; set; }
        public string Ma { get; set; }
        public bool? LaDuocPhamBenhVien { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string DVT { get; set; }
        public string SangDisplay { get; set; }
        public string TruaDisplay { get; set; }
        public string ChieuDisplay { get; set; }
        public string ToiDisplay { get; set; }
        public int? SoNgayDung { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public string ThoiGianDungSangDisplay { get; set; }
        public string ThoiGianDungTruaDisplay { get; set; }
        public string ThoiGianDungChieuDisplay { get; set; }
        public string ThoiGianDungToiDisplay { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public double? SoLuong { get; set; }
        public string SoLuongDisplay { get; set; }
        public string TenDuongDung { get; set; }
        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public decimal ThanhTien => DonGia * (decimal)SoLuong.GetValueOrDefault();
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public string ThuocBHYT { get; set; }
        public string TuongTacThuoc { get; set; }
        public string DiUngThuocDisplay { get; set; }
        public string GhiChu { get; set; }
        public string GhiChuDonThuoc { get; set; }
        public int NhomId { get; set; }
        public string Nhom { get; set; }
        public EnumLoaiDonThuoc LoaiDonThuoc { get; set; }
        public bool ChuaThanhToan { get; set; }
        public bool CheckBox { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }
        public string TooltipTiLeBHYT => TiLeThanhToanBHYT?.Replace("\n", "<br>");
    }

    public class NoiTruDonThuocChiTietVo
    {
        public long NoiTruDonThuocChiTietId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long DuocPhamId { get; set; }
        public double SoLuong { get; set; }
        public int? SoNgayDung { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public double? DungSang { get; set; }
        public double? DungTrua { get; set; }
        public double? DungChieu { get; set; }
        public double? DungToi { get; set; }
        public Enums.EnumLoaiDonThuoc LoaiDonThuoc => LoaiKhoThuoc == LoaiKhoThuoc.ThuocBHYT
            ? Enums.EnumLoaiDonThuoc.ThuocBHYT
            : Enums.EnumLoaiDonThuoc.ThuocKhongBHYT;
        public LoaiKhoThuoc LoaiKhoThuoc { get; set; }
        public string GhiChu { get; set; }
    }

    public class ThongTinThuocNoiTruVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public long DuocPhamId { get; set; }
        public long LoaiDuocPham { get; set; }

    }

    public class InToaThuocRaVien
    {
        public InToaThuocRaVien()
        {
            Ids = new List<long>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public string HostingName { get; set; }
        public string GhiChu { get; set; }
        public bool Header { get; set; }
        public List<long> Ids { get; set; }

    }
    public class InNoiTruDonThuocChiTietVo : GridItem
    {
        public int? STT { get; set; }
        public string Ten { get; set; }
        public string MaHoatChat { get; set; }
        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public string TenDuongDung { get; set; }
        public string DVT { get; set; }
        public double SoLuong { get; set; }
        public int? SoNgayDung { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public string ThoiGianDungSangDisplay => ThoiGianDungSang?.ConvertIntSecondsToTime12h();
        public string ThoiGianDungTruaDisplay => ThoiGianDungTrua?.ConvertIntSecondsToTime12h();
        public string ThoiGianDungChieuDisplay => ThoiGianDungChieu?.ConvertIntSecondsToTime12h();
        public string ThoiGianDungToiDisplay => ThoiGianDungToi?.ConvertIntSecondsToTime12h();
        public double? DungSang { get; set; }
        public double? DungTrua { get; set; }
        public double? DungChieu { get; set; }
        public double? DungToi { get; set; }
        public LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public string DungSangDisplay
        {
            get
            {
                var result = string.Empty;
                if (DungSang != null)
                {
                    if (LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan)
                    {
                        result = NumberHelper.ChuyenSoRaText(DungSang.GetValueOrDefault(), false);
                    }
                    else
                    {
                        result = DungSang.FloatToStringFraction() + " ";
                    }
                }
                if (ThoiGianDungSang != null)
                {
                    result += "(" + ThoiGianDungSang?.ConvertIntSecondsToTime12h() + ")";
                }
                return result;
            }
        }

        public string DungTruaDisplay
        {
            get
            {
                var result = string.Empty;
                if (DungTrua != null)
                {
                    if (LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan)
                    {
                        result = NumberHelper.ChuyenSoRaText(DungTrua.GetValueOrDefault(), false);
                    }
                    else
                    {
                        result = DungTrua.FloatToStringFraction() + " ";
                    }
                }
                if (ThoiGianDungTrua != null)
                {
                    result += "(" + ThoiGianDungTrua?.ConvertIntSecondsToTime12h() + ")";
                }
                return result;
            }
        }
        public string DungChieuDisplay
        {
            get
            {
                var result = string.Empty;
                if (DungChieu != null)
                {
                    if (LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan)
                    {
                        result = NumberHelper.ChuyenSoRaText(DungChieu.GetValueOrDefault(), false);
                    }
                    else
                    {
                        result = DungChieu.FloatToStringFraction() + " ";
                    }
                }
                if (ThoiGianDungChieu != null)
                {
                    result += "(" + ThoiGianDungChieu?.ConvertIntSecondsToTime12h() + ")";
                }
                return result;
            }
        }
        public string DungToiDisplay
        {
            get
            {
                var result = string.Empty;
                if (DungToi != null)
                {
                    if (LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.GayNghien || LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.HuongThan)
                    {
                        result = NumberHelper.ChuyenSoRaText(DungToi.GetValueOrDefault(), false);
                    }
                    else
                    {
                        result = DungToi.FloatToStringFraction() + " ";
                    }
                }
                if (ThoiGianDungToi != null)
                {
                    result += "(" + ThoiGianDungToi?.ConvertIntSecondsToTime12h() + ")";
                }
                return result;
            }
        }
        public DateTime ThoiDiemKeDon { get; set; }
        public string GhiChu { get; set; }
        public string CachDung { get; set; }
        public bool LaDuocPhamBenhVien { get; set; }
        public string TenBacSiKeDon { get; set; }
        public long BacSiKeDonId { get; set; }

        //public bool? LaThucPhamChucNang { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public long? DuocPhamBenhVienPhanNhomChaId { get; set; }

        public long DuongDungId { get; set; }
        public int DuongDungNumber
        {
            get
            {
                //BVHD-3959
                return BenhVienHelper.GetSoThuThuocTheoDuongDung(DuongDungId);
                //var numberSort = 0;
                //numberSort = DuongDungId == 12 ? 1 : (DuongDungId == 1 ? 2 : (DuongDungId == 26 ? 3 : (DuongDungId == 22 ? 4 : 5)));
                //return numberSort;
            }
        }
    }


    public class PhieuDieuTriPhaThuocTiemGridVo : GridItem
    {
        public PhieuDieuTriPhaThuocTiemGridVo()
        {
            SoLuongDisplay = new List<double>();
            DonGias = new List<decimal>();
            ThanhTiens = new List<decimal>();
        }
        public int? SoThuTu { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public long? NoiTruChiDinhPhaThuocTiemId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long? KhoId { get; set; }
        public string TenKho { get; set; }
        public string HoatChat { get; set; }
        public string HamLuong { get; set; }
        public string DVT { get; set; }
        public string TenDuongDung { get; set; }
        public double SoLuong { get; set; }
        public List<double> SoLuongDisplay { get; set; }
        public List<decimal> DonGias { get; set; }
        public List<decimal> ThanhTiens { get; set; }
        public decimal ThanhTien => ThanhTiens.Sum(x => x);
        public string GhiChu { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public int NhomId => LaDuocPhamBHYT ? 1 : 0;
        public int LaDuocPhamBHYTNumber => LaDuocPhamBHYT ? 2 : 1;
        public string Nhom => LaDuocPhamBHYT ? "BHYT" : "Không BHYT";
        public bool TinhTrang { get; set; }
        public string TinhTrangDisplay => TinhTrang ? "Đã xuất" : "Chưa xuất";
        public string PhieuLinh { get; set; }
        public string PhieuXuat { get; set; }
        public bool CoYeuCauTraDuocPhamTuBenhNhanChiTiet { get; set; }
        public bool LaTuTruc { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public bool? LaThuocHuongThanGayNghien { get; set; }
        public double? CachGioTiem { get; set; }
        public bool? LaDichTruyen { get; set; }
        public string GioSuDung { get; set; }
        public int? ThoiGianBatDauTiem { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public string TenNhanVienChiDinh { get; set; }
        public string ThoiDiemChiDinhDisplay => ThoiDiemChiDinh?.ApplyFormatDateTime();
        public int? TheTich { get; set; }
        public int KhuVuc { get; set; }
        public long DuongDungId { get; set; }
        public LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public int DuongDungNumber
        {
            get
            {
                //BVHD-3959
                return BenhVienHelper.GetSoThuThuocTheoDuongDung(DuongDungId);
                //var numberSort = 0;
                //if (LoaiThuocTheoQuanLy != null && LoaiThuocTheoQuanLy == Enums.LoaiThuocTheoQuanLy.ThuocDoc)
                //{
                //    numberSort = 1;
                //}
                //else
                //{
                //    numberSort = DuongDungId == 12 ? 2 : (DuongDungId == 1 ? 3 : (DuongDungId == 26 ? 4 : (DuongDungId == 22 ? 5 : 6)));
                //}
                //return numberSort;
            }
        }
        public int? SoLanTrenMui { get; set; }
        public double? SoLanTrenNgay { get; set; }
    }

    public class PhaThuocTiemBenhVienVo : GridItem
    {
        public PhaThuocTiemBenhVienVo()
        {
            NoiTruChiDinhDuocPhams = new List<PhaThuocTiemBenhVienChiTietVo>();
        }
        public int? SoLanTrenNgay { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long? PhieuDieuTriHienTaiId { get; set; }
        public bool? LaDichTruyen { get; set; }
        public int? TheTich { get; set; }
        public int? SoLanTrenMui { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public int? SoLanTrenVien { get; set; }
        public double? CachGioDungThuoc { get; set; }
        public double? LieuDungTrenNgay { get; set; }
        public double? CachGioTiem { get; set; }
        public string GhiChu { get; set; }
        public int? ThoiGianBatDauTiem { get; set; }
        public List<PhaThuocTiemBenhVienChiTietVo> NoiTruChiDinhDuocPhams { get; set; }

        // truyền
        public int? TocDoTruyen { get; set; }
        public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
        public int? SoThuTu { get; set; }
        public double? CachGioTruyen { get; set; }
    }

    public class PhaThuocTiemBenhVienChiTietVo : GridItem
    {
        public long? KhoId { get; set; }
        public EnumLoaiKhoDuocPham? LoaiKho { get; set; }
        public bool? LaTuTruc { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public int? LaDuocPhamBHYT { get; set; }
        public int? LaDuocPhamBHYTNumber { get; set; }
        public double? SoLuong { get; set; }
        public int? KhuVuc { get; set; }
    }

    public class ThuocBenhVienTangGiamSTTTiemHoacTruyenVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public long? PhieuDieuTriHienTaiId { get; set; }
        public bool LaThuocTiem { get; set; }
        public bool? LaTangSTT { get; set; }

    }

    public class CapNhatKhongTinhPhiTiem : GridItem
    {
        public CapNhatKhongTinhPhiTiem()
        {
            Ids = new List<long>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public bool KhongTinhPhi { get; set; }
        public bool LaThuocTiem { get; set; }
        public List<long> Ids { get; set; }
    }



    public class PhieuDieuTriPhaThuocTruyenGridVo : GridItem
    {
        public PhieuDieuTriPhaThuocTruyenGridVo()
        {
            SoLuongDisplay = new List<double>();
            DonGias = new List<decimal>();
            ThanhTiens = new List<decimal>();
        }
        public int? STT { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public string Ma { get; set; }
        public long? NoiTruChiDinhPhaThuocTruyenId { get; set; }
        public string Ten { get; set; }
        public long? KhoId { get; set; }
        public string TenKho { get; set; }
        public string HoatChat { get; set; }
        public string HamLuong { get; set; }
        public string DVT { get; set; }
        public string TenDuongDung { get; set; }
        public double SoLuong { get; set; }
        public List<double> SoLuongDisplay { get; set; }
        public List<decimal> DonGias { get; set; }
        public List<decimal> ThanhTiens { get; set; }
        public decimal ThanhTien => ThanhTiens.Sum(x => x);
        public string GhiChu { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public int NhomId => LaDuocPhamBHYT ? 1 : 0;
        public string Nhom => LaDuocPhamBHYT ? "BHYT" : "Không BHYT";
        public bool TinhTrang { get; set; }
        public string TinhTrangDisplay => TinhTrang ? "Đã xuất" : "Chưa xuất";
        public string PhieuLinh { get; set; }
        public string PhieuXuat { get; set; }
        public bool CoYeuCauTraDuocPhamTuBenhNhanChiTiet { get; set; }
        public bool LaTuTruc { get; set; }
        public bool? LaDichTruyen { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public bool? LaThuocHuongThanGayNghien { get; set; }
        public string GioSuDung { get; set; }
        public int? TocDoTruyen { get; set; }
        public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        public string DonViTocDoTruyenDisplay => DonViTocDoTruyen?.GetDescription();
        public double? CachGioTruyen { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public string TenNhanVienChiDinh { get; set; }
        public string ThoiDiemChiDinhDisplay => ThoiDiemChiDinh?.ApplyFormatDateTime();
        public int? TheTich { get; set; }
        public int KhuVuc { get; set; }
        //BVHD-3959
        public int DuongDungNumber => BenhVienHelper.GetSoThuThuocTheoDuongDung(DuongDungId);
        public long DuongDungId { get; set; }
        public double? SoLanTrenNgay { get; set; }

    }

    public class PhaThuocTruyenBenhVienVo : GridItem
    {
        public PhaThuocTruyenBenhVienVo()
        {
            NoiTruChiDinhDuocPhams = new List<PhaThuocTiemBenhVienChiTietVo>();
        }
        public int? SoLanDungTrongNgay { get; set; }
        public int? SoLanTrenNgay { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public double? CachGioDungThuoc { get; set; }
        public double? LieuDungTrenNgay { get; set; }
        public long? PhieuDieuTriHienTaiId { get; set; }
        public bool? LaDichTruyen { get; set; }
        public int? TheTich { get; set; }
        public int? TocDoTruyen { get; set; }
        public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public EnumLoaiKhoDuocPham? LoaiKho { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
        public string GhiChu { get; set; }
        public int? SoThuTu { get; set; }
        public double? CachGioTruyen { get; set; }
        public List<PhaThuocTiemBenhVienChiTietVo> NoiTruChiDinhDuocPhams { get; set; }
    }

    public class NoiTruDonThuocTongHopVo : GridItem
    {
        public NoiTruDonThuocTongHopVo()
        {
            NoiTruDonThuocTongHopChiTietVos = new List<NoiTruDonThuocTongHopChiTietVo>();
            NoiTruDonThuocTuVanChiTietVos = new List<NoiTruDonThuocTongHopChiTietVo>();
            Dates = new List<DateTime>();
        }
        public List<DateTime> Dates { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        //public long PhieuDieuTriId { get; set; }
        public long? KhoaId { get; set; }
        public List<NoiTruDonThuocTongHopChiTietVo> NoiTruDonThuocTongHopChiTietVos { get; set; }
        public List<NoiTruDonThuocTongHopChiTietVo> NoiTruDonThuocTuVanChiTietVos { get; set; }
    }

    public class KetQuaApDungNoiTruDonThuocTongHopVo
    {
        public KetQuaApDungNoiTruDonThuocTongHopVo()
        {
            NoiTruDonThuocTongHopChiTietVos = new List<NoiTruDonThuocTongHopChiTietVo>();
            NoiTruDonThuocTuVanChiTietVos = new List<NoiTruDonThuocTongHopChiTietVo>();
        }
        public bool ThanhCong { get; set; }
        public string Error { get; set; }
        public List<NoiTruDonThuocTongHopChiTietVo> NoiTruDonThuocTongHopChiTietVos { get; set; }
        public List<NoiTruDonThuocTongHopChiTietVo> NoiTruDonThuocTuVanChiTietVos { get; set; }

    }



    public class NoiTruDonThuocTongHopChiTietVo : GridItem
    {
        public NoiTruDonThuocTongHopChiTietVo Clone()
        {
            return (NoiTruDonThuocTongHopChiTietVo)this.MemberwiseClone();
        }
        public long? NoiTruChiDinhPhaThuocTruyenId { get; set; }
        public long? NoiTruChiDinhPhaThuocTiemId { get; set; }
        public long? KhoId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenDuongDung { get; set; }
        public string HoatChat { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public int NhomId => LaDuocPhamBHYT ? 1 : 0;
        public string Nhom => LaDuocPhamBHYT ? "BHYT" : "Không BHYT";
        public double SoLuong { get; set; }
        public int? SoLanDungTrongNgay { get; set; }
        public string DungSang { get; set; }
        public string DungTrua { get; set; }
        public string DungChieu { get; set; }
        public string DungToi { get; set; }
        public int? ThoiGianDungSang { get; set; }
        public int? ThoiGianDungTrua { get; set; }
        public int? ThoiGianDungChieu { get; set; }
        public int? ThoiGianDungToi { get; set; }
        public string ThoiGianDungSangDisplay => ThoiGianDungSang != null ? "(" + ThoiGianDungSang?.ConvertIntSecondsToTime12h() + ")" : "";
        public string ThoiGianDungTruaDisplay => ThoiGianDungTrua != null ? "(" + ThoiGianDungTrua?.ConvertIntSecondsToTime12h() + ")" : "";
        public string ThoiGianDungChieuDisplay => ThoiGianDungChieu != null ? "(" + ThoiGianDungChieu?.ConvertIntSecondsToTime12h() + ")" : "";
        public string ThoiGianDungToiDisplay => ThoiGianDungToi != null ? "(" + ThoiGianDungToi?.ConvertIntSecondsToTime12h() + ")" : "";
        public string GhiChu { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public bool? LaDichTruyen { get; set; }
        public int? TheTich { get; set; }
        public int? TocDoTruyen { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public string NgayDieuTriDisplay => NgayDieuTri.ApplyFormatDate();

        public int? ThoiGianBatDauTruyen { get; set; }
        public double? CachGioTruyenDich { get; set; }
        public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        public long? PhieuDieuTriHienTaiId { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public EnumLoaiKhoDuocPham? LoaiKho { get; set; }
        public int? SoLanTrenVien { get; set; }
        public double? CachGioDungThuoc { get; set; }
        public double? LieuDungTrenNgay { get; set; }
        public int? ThoiGianBatDauTiem { get; set; }
        public int? SoLanTrenMui { get; set; }
        public double? CachGioTiem { get; set; }
        public int? SoThuTu { get; set; }
        public int? SoLanTrenNgay { get; set; }
        public double? CachGioTruyen { get; set; }
        public double SoLuongTonKho { get; set; }
        //public long NhanVienChiDinhId { get; set; }
        //public long NoiChiDinhId { get; set; }
        //public EnumYeuCauDuocPhamBenhVien TrangThai { get; set; }
        //public EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }
        public bool? LaNoiTruDonThuocTuVan { get; set; }
        public string GioSuDung
        {
            get
            {
                var gioSuDung = string.Empty;
                double seconds = 3600;
                if (LaDichTruyen == true)
                {
                    if (NoiTruChiDinhPhaThuocTiemId != null)
                    {
                        var thoiGianBatDauTiem = ThoiGianBatDauTiem;
                        if (thoiGianBatDauTiem != null && SoLanTrenNgay != null && CachGioTiem != null)
                        {
                            var time = thoiGianBatDauTiem?.ConvertIntSecondsToTime12h();
                            thoiGianBatDauTiem += (int?)(CachGioTiem * seconds);
                            gioSuDung += time + "; ";
                        }
                    }
                    else if (NoiTruChiDinhPhaThuocTruyenId != null)
                    {
                        var thoiGianBatDauTruyen = ThoiGianBatDauTruyen;
                        if (thoiGianBatDauTruyen != null && SoLanTrenNgay != null && CachGioTruyen != null)
                        {
                            var time = thoiGianBatDauTruyen?.ConvertIntSecondsToTime12h();
                            thoiGianBatDauTruyen += (int?)(CachGioTruyen * seconds);
                            gioSuDung += time + "; ";
                        }
                    }
                    else
                    {
                        var thoiGianBatDauTruyen = ThoiGianBatDauTruyen;
                        if (thoiGianBatDauTruyen != null && SoLanTrenNgay != null && CachGioTruyenDich != null)
                        {
                            var time = thoiGianBatDauTruyen?.ConvertIntSecondsToTime12h();
                            thoiGianBatDauTruyen += (int?)(CachGioTruyenDich * seconds);
                            gioSuDung += time + "; ";
                        }
                    }

                }
                return gioSuDung;
            }
        }

    }

    public class NoiTruDonVTYTTongHopVo : GridItem
    {
        public NoiTruDonVTYTTongHopVo()
        {
            NoiTruDonVTYTTongHopChiTietVos = new List<NoiTruDonVTYTTongHopChiTietVo>();
            Dates = new List<DateTime>();
        }
        public List<DateTime> Dates { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long? KhoaId { get; set; }
        public List<NoiTruDonVTYTTongHopChiTietVo> NoiTruDonVTYTTongHopChiTietVos { get; set; }
    }

    public class NoiTruDonVTYTTongHopChiTietVo : GridItem
    {
        public NoiTruDonVTYTTongHopChiTietVo Clone()
        {
            return (NoiTruDonVTYTTongHopChiTietVo)this.MemberwiseClone();
        }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long KhoId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long VatTuBenhVienId { get; set; }
        public int NhomId => LaVatTuBHYT ? 1 : 0;
        public string Nhom => LaVatTuBHYT ? "BHYT" : "Không BHYT";
        public bool LaTuTruc { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public string NgayDieuTriDisplay => NgayDieuTri.ApplyFormatDate();
        public string DVT { get; set; }
        public string TenDVKT { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public double SoLuong { get; set; }
        public double SoLuongTonKho { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long PhieuDieuTriHienTaiId { get; set; }
        public EnumLoaiKhoDuocPham? LoaiKho { get; set; }

    }

    public class KetQuaApDungNoiTruDonVTYTTongHopVo
    {
        public KetQuaApDungNoiTruDonVTYTTongHopVo()
        {
            NoiTruDonVTYTTongHopChiTietVos = new List<NoiTruDonVTYTTongHopChiTietVo>();
        }
        public bool ThanhCong { get; set; }
        public string Error { get; set; }
        public List<NoiTruDonVTYTTongHopChiTietVo> NoiTruDonVTYTTongHopChiTietVos { get; set; }

    }
    public class ApDungThoiGianDienBienThuocVo
    {
        public ApDungThoiGianDienBienThuocVo()
        {
            DataGridDichVuChons = new List<PhieuDieuTriThuocGridVo>();
        }
        public List<PhieuDieuTriThuocGridVo> DataGridDichVuChons { get; set; }

        public DateTime? ThoiGianDienBien { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long PhieuDieuTriId { get; set; }
    }

}
