using System;
using System.Collections.Generic;
using Camino.Api.Models.YeuCauKhamBenhChanDoanPhanBiet;
using Camino.Api.Models.YeuCauKhamBenhKhamBoPhanKhac;
using Camino.Core.Domain;
using Camino.Core.Helpers;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauKhamBenhViewModel : BaseViewModel
    {
        public KhamBenhYeuCauKhamBenhViewModel()
        {
            YeuCauDichVuKyThuats = new List<KhamBenhYeuCauDichVuKyThuatViewModel>();
            YeuCauKhamBenhDonThuocs = new List<KhamBenhYeuCauDonThuocViewModel>();
            YeuCauKhamBenhDonVTYTs = new List<KhamBenhYeuCauDonVTYTViewModel>();
            YeuCauKhamBenhTrieuChungs = new List<YeuCauKhamBenhTrieuChungViewModel>();
            YeuCauKhamBenhChuanDoans = new List<YeuCauKhamBenhChuanDoanViewModel>();
            //ICDPhuIds = new List<long>();
            YeuCauKhamBenhICDKhacs = new List<YeuCauKhamBenhICDKhacViewModel>();
            YeuCauKhamBenhBoPhanTonThuongs = new List<YeuCauKhamBenhBoPhanTonThuongViewModel>();
            TemplateDichVuKhamSucKhoes = new List<KhamBenhTemplateDichVuKhamSucKhoeViewModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public long? NhomGiaDichVuKhamBenhBenhVienId { get; set; }

        public long? YeuCauKhamBenhTruocId { get; set; }
        public string MaDichVu { get; set; }
        public string MaDichVuTT37 { get; set; }

        public string LoiDanCuaBacSi { get; set; }

        public string TenDichVu { get; set; }
        public string TenDichVuDisplay { get; set; }
        public decimal? Gia { get; set; }
        public string TenLoaiGia { get; set; }
        public string TenNoiDangKyThucHien { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }

        public int? TiLeUuDai { get; set; }
        public long? GoiDichVuId { get; set; }
        public int? TiLeChietKhau { get; set; }
        public string MoTa { get; set; }
        public bool? DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public decimal? GiaBaoHiemThanhToan { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh? TrangThai { get; set; }
        public Enums.TrangThaiThanhToan? TrangThaiThanhToan { get; set; }

        public long? NhanVienChiDinhId { get; set; }
        public long? NoiChiDinhId { get; set; }
        public DateTime? ThoiDiemChiDinh { get; set; }
        public bool? DaThanhToan { get; set; }
        public long? NoiThanhToanId { get; set; }
        public long? NhanVienThanhToanId { get; set; }
        public DateTime? ThoiDiemThanhToan { get; set; }
        public DateTime? ThoiDiemDangKy { get; set; }
        public long? NoiDangKyId { get; set; }
        public long? BacSiDangKyId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public long? NoiThucHienId { get; set; }
        public long? BacSiThucHienId { get; set; }
        public string BenhSu { get; set; }
        public string ThongTinKhamTheoKhoa { get; set; }
        public long? IcdchinhId { get; set; }
        public bool? CoKhamChuyenKhoaTiepTheo { get; set; }
        public bool? CoKeToa { get; set; }
        public bool? CoTaiKham { get; set; }

        public string TenICDChinh { get; set; }
        //public List<long> ICDPhuIds { get; set; }

        public string TenBenh { get; set; }
        public string GhiChu { get; set; }

        public string BacSiNoiDangKyId { get; set; }

        public long? NoiKetLuanId { get; set; }
        public long? BacSiKetLuanId { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }
        public Enums.EnumKetQuaDieuTri? KetQuaDieuTri { get; set; }

        public string TenKetQuaDieuTri
        {
            get { return KetQuaDieuTri == null ? null : KetQuaDieuTri.GetDescription(); }
        }

        public Enums.EnumTinhTrangRaVien? TinhTrangRaVien { get; set; }

        public string TenTinhTrangRaVien
        {
            get { return TinhTrangRaVien == null ? null : TinhTrangRaVien.GetDescription(); }
        }

        public string HuongDieuTri { get; set; }
        //public bool? CoKhamChuyenKhoaTiepTheo { get; set; }
        //public bool? CoKeToa { get; set; }
        //public bool? CoTaiKham { get; set; }
        public DateTime? NgayTaiKham { get; set; }
        public string GhiChuTaiKham { get; set; }
        public string GhiChuTrieuChungLamSang { get; set; }
        public bool? QuayLaiYeuCauKhamBenhTruoc { get; set; }
        //public KetLuanKhamBenhViewModel KetLuanKhamBenh { get; set; }

        public string KhamToanThan { get; set; }
        public string TuanHoan { get; set; }
        public string HoHap { get; set; }
        public string TieuHoa { get; set; }
        public string ThanTietNieuSinhDuc { get; set; }
        public string ThanKinh { get; set; }
        public string CoXuongKhop { get; set; }
        public string TaiMuiHong { get; set; }
        public string RangHamMat { get; set; }
        public string NoiTietDinhDuong { get; set; }
        public string SanPhuKhoa { get; set; }
        public string DaLieu { get; set; }
        public long? ChanDoanSoBoICDId { get; set; }
        public string TenChanDoanSoBoICD { get; set; }
        public string ChanDoanSoBoGhiChu { get; set; }

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
        public string ThongTinKhamTheoDichVuTemplate { get; set; }
        public string ThongTinKhamTheoDichVuData { get; set; }

        public bool? LaThamVan { get; set; }
        public bool? CoDieuTriNgoaiTru { get; set; }
        public string ChanDoanCuaNoiGioiThieu { get; set; }
        public bool? KhongKeToa { get; set; }
        public string TinhTrangBenhNhanChuyenVien { get; set; }
        public Enums.LyDoChuyenTuyen? LoaiLyDoChuyenVien { get; set; }
        public DateTime? ThoiDiemChuyenVien { get; set; }
        public string PhuongTienChuyenVien { get; set; }
        public long? NhanVienHoTongChuyenVienId { get; set; }
        public string HoTenNhanVienHoTong { get; set; }
        public decimal SoDuTaiKhoan { get; set; }
        public string SoDuTaiKhoanDisplay
        {
            get { return SoDuTaiKhoan.ApplyFormatMoneyVND(); }
        }

        public decimal SoDuTaiKhoanConLai { get; set; }
        public string SoDuTaiKhoanConLaiDisplay
        {
            get { return SoDuTaiKhoanConLai.ApplyFormatMoneyVND(); }
        }
        public decimal MucTranChiPhi { get; set; }
        public string GhiChuCanLamSang { get; set; }

        //BVHD-3574
        public string NoiDungKhamBenh { get; set; }

        public Enums.ChuyenKhoaKhamSucKhoe? ChuyenKhoaKhamSucKhoe { get; set; }

        //BVHD-3706
        public string TrieuChungTiepNhan { get; set; }
        public string KetQuaXetNghiemCLS { get; set; }
        public string PhuongPhapTrongDieuTri { get; set; }

        public DateTime? NghiHuongBHXHTuNgay { get; set; }
        public DateTime? NghiHuongBHXHDenNgay { get; set; }
        public long? NghiHuongBHXHNguoiInId { get; set; }

        //BVHD-3575
        public bool? LaChiDinhTuNoiTru { get; set; }
        public string SoBenhAn { get; set; }

        public KhamBenhYeuCauKhamBenhViewModel YeuCauKhamBenhTruoc { get; set; }
        public KhamBenhYeuCauKhamBenhViewModel YeuCauKhamBenhTiepTheo { get; set; }
        public YeuCauDichVuKyThuatViewModel YeuCauDichVuKyThuat { get; set; }

        public KhamBenhTemplateKhamBenhTheoDichVuViewModel TemplateKhamBenhTheoDichVu { get; set; }

        public List<KhamBenhYeuCauDichVuKyThuatViewModel> YeuCauDichVuKyThuats { get; set; }
        public List<KhamBenhYeuCauDonThuocViewModel> YeuCauKhamBenhDonThuocs { get; set; }
        public List<KhamBenhYeuCauDonVTYTViewModel> YeuCauKhamBenhDonVTYTs { get; set; }

        public List<YeuCauKhamBenhICDKhacViewModel> YeuCauKhamBenhICDKhacs { get; set; }

        public List<YeuCauKhamBenhTrieuChungViewModel> YeuCauKhamBenhTrieuChungs { get; set; }

        public List<YeuCauKhamBenhChuanDoanViewModel> YeuCauKhamBenhChuanDoans { get; set; }

        public List<YeuCauKhamBenhKhamBoPhanKhacViewModel> YeuCauKhamBenhKhamBoPhanKhacs { get; set; }
        public List<YeuCauKhamBenhChanDoanPhanBietViewModel> YeuCauKhamBenhChanDoanPhanBiets { get; set; }
        public List<YeuCauKhamBenhBoPhanTonThuongViewModel> YeuCauKhamBenhBoPhanTonThuongs { get; set; }
        public List<KhamBenhTemplateDichVuKhamSucKhoeViewModel> TemplateDichVuKhamSucKhoes { get; set; }
    }

}
