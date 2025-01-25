using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.BHYT;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DichVuBenhVienTongHops;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.HinhThucDens;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.MayXetNghiems;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;

namespace Camino.Services.BaoCao
{
    [ScopedDependency(ServiceType = typeof(IBaoCaoService))]
    public partial class BaoCaoService : IBaoCaoService
    {
        IRepository<YeuCauNhapKhoDuocPham> _yeuCauNhapKhoDuocPhamRepository;
        IRepository<YeuCauNhapKhoVatTu> _yeuCauNhapKhoVatTuRepository;
        IRepository<DuocPhamBenhVienPhanNhom> _duocPhamBenhVienPhanNhomRepository;
        IRepository<Core.Domain.Entities.NhaThaus.NhaThau> _nhaThauRepository;
        IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        IRepository<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTriRepository;
        IRepository<XuatKhoDuocPham> _xuatKhoDuocPhamRepository;
        IRepository<XuatKhoVatTu> _xuatKhoVatTuRepository;
        IRepository<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTriRepository;
        IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        IRepository<Camino.Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> _noiTruBenhAnRepository;
        IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _KhoaPhongRepository;
        IRepository<Kho> _khoRepository;
        IRepository<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThuRepository;
        IRepository<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChiRepository;
        IRepository<Template> _templateRepository;
        IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;
        IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        IRepository<YeuCauDichVuKyThuatKhamSangLocTiemChung> _yeuCauDichVuKyThuatKhamSangLocTiemChungRepository;
        IRepository<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository;
        IRepository<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienRepository;
        IRepository<YeuCauVatTuBenhVien> _yeuCauVatTuBenhVienRepository;
        IRepository<YeuCauTruyenMau> _yeuCauTruyenMauRepository;
        IRepository<DonThuocThanhToan> _donThuocThanhToanRepository;
        IRepository<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTietRepository;
        IRepository<DonVTYTThanhToanChiTiet> _donVTYTThanhToanChiTietRepository;
        IRepository<DonThuocThanhToanChiTietTheoPhieuThu> _donThuocThanhToanChiTietTheoPhieuThuRepository;
        IRepository<DonVTYTThanhToanChiTietTheoPhieuThu> _donVTYTThanhToanChiTietTheoPhieuThuRepository;
        IRepository<DichVuXetNghiemKetNoiChiSo> _dichVuXetNghiemKetNoiChiSoRepository;
        IRepository<PhienXetNghiem> _phienXetNghiemRepository;
        IRepository<HopDongKhamSucKhoe> _hopDongKhamSucKhoeRepository;
        IRepository<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeNhanVienRepository;
        IUserAgentHelper _userAgentHelper;
        ICauHinhService _cauHinhService;
        IRepository<Core.Domain.Entities.KhamDoans.CongTyKhamSucKhoe> _congTyKhamSucKhoeRepository;
        IRepository<GiuongBenh> _giuongBenhRepository;
        IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        IRepository<ICD> _icdRepository;
        IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        IRepository<PhauThuatThuThuatEkipBacSi> _phauThuatThuThuatEkipBacSiRepository;
        IRepository<PhauThuatThuThuatEkipDieuDuong> _phauThuatThuThuatEkipDieuDuongRepository;
        IRepository<YeuCauGoiDichVu> _yeuCauGoiDichVuRepository;
        IRepository<Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu> _noiGioiThieuRepository;
        IRepository<BenhNhan> _benhNhanRepository;
        IRepository<HinhThucDen> _hinhThucDenRepository;
        IRepository<TaiKhoanBenhNhanChiThongTin> _taiKhoanBenhNhanChiThongTinRepository;
        IRepository<DichVuKhamBenhBenhVien> _dichVuKhamBenhBenhVienRepository;
        IRepository<NoiTruKhoaPhongDieuTri> _noiTruKhoaPhongDieuTriRepository;
        IRepository<Core.Domain.Entities.MauVaChePhams.MauVaChePham> _mauVaChePhamRepository;
        IRepository<DichVuBenhVienTongHop> _dichVuBenhVienTongHopRepository;
        IRepository<NoiTruPhieuDieuTri> _noiTruPhieuDieuTriRepository;
        IRepository<YeuCauKhamBenhDonThuocChiTiet> _yeuCauKhamBenhDonThuocChiTietRepository;
        IRepository<YeuCauDichVuGiuongBenhVienChiPhiBHYT> _yeuCauDichVuGiuongBenhVienChiPhiBhytRepository;
        IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        IRepository<ChuongICD> _chuongICDRepository;
        IRepository<YeuCauTraDuocPhamChiTiet> _yeuCauTraDuocPhamChiTietRepository;
        IRepository<PhienXetNghiemChiTiet> _phienXetNghiemChiTietRepository;
        IRepository<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTietRepository;
        IRepository<NhomICDTheoBenhVien> _nhomICDTheoBenhVienRepository;
        IRepository<MayXetNghiem> _mayXetNghiemRepository;
        IRepository<User> _userRepository;
        private readonly IRepository<GoiKhamSucKhoeDichVuKhamBenh> _goiKhamSucKhoeDichVuKhamBenhRepository;
        private readonly IRepository<GoiKhamSucKhoeDichVuDichVuKyThuat> _goiKhamSucKhoeDichVuKyThuatRepository;
        IRepository<ThuePhong> _thuePhongRepository;
        private readonly IRepository<YeuCauTiepNhanDuLieuGuiCongBHYT> _yeuCauTiepNhanDuLieuGuiCongBHYT;
        IRepository<MienGiamChiPhi> _mienGiamChiPhiRepository;
        IRepository<CongTyBaoHiemTuNhan> _congTyBaoHiemTuNhanRepository;
        IRepository<NoiGioiThieuHopDong> _noiGioiThieuHopDongRepository;
        public BaoCaoService(IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository, IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<YeuCauNhapKhoDuocPham> yeuCauNhapKhoDuocPhamRepository,
            IRepository<YeuCauNhapKhoVatTu> yeuCauNhapKhoVatTuRepository,
            IRepository<DuocPhamBenhVienPhanNhom> duocPhamBenhVienPhanNhomRepository,
            IRepository<Core.Domain.Entities.NhaThaus.NhaThau> nhaThauRepository,
            IRepository<TaiKhoanBenhNhanThu> taiKhoanBenhNhanThuRepository, IRepository<TaiKhoanBenhNhanChi> taiKhoanBenhNhanChiRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
             IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> KhoaPhongRepository,
             IRepository<Kho> khoRepository, IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository, IRepository<XuatKhoDuocPhamChiTietViTri> xuatKhoDuocPhamChiTietViTriRepository,
             IRepository<XuatKhoDuocPham> xuatKhoDuocPhamRepository, IRepository<XuatKhoVatTu> xuatKhoVatTuRepository,
             IRepository<Template> templateRepository,
             IUserAgentHelper userAgentHelper,
             IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
             IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
            IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository,
            IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository,
            IRepository<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository,
            IRepository<YeuCauDuocPhamBenhVien> yeuCauDuocPhamBenhVienRepository,
            IRepository<YeuCauDichVuKyThuatKhamSangLocTiemChung> yeuCauDichVuKyThuatKhamSangLocTiemChungRepository,
            IRepository<YeuCauVatTuBenhVien> yeuCauVatTuBenhVienRepository,
            IRepository<YeuCauTruyenMau> yeuCauTruyenMauRepository,
            IRepository<DonThuocThanhToan> donThuocThanhToanRepository, IRepository<DonThuocThanhToanChiTiet> donThuocThanhToanChiTietRepository, IRepository<DonVTYTThanhToanChiTiet> donVTYTThanhToanChiTietRepository,
            IRepository<DonThuocThanhToanChiTietTheoPhieuThu> donThuocThanhToanChiTietTheoPhieuThuRepository, IRepository<DonVTYTThanhToanChiTietTheoPhieuThu> donVTYTThanhToanChiTietTheoPhieuThuRepository,
            IRepository<DichVuXetNghiemKetNoiChiSo> dichVuXetNghiemKetNoiChiSoRepository,
            IRepository<PhienXetNghiem> phienXetNghiemRepository,
            IRepository<XuatKhoVatTuChiTietViTri> xuatKhoVatTuChiTietViTriRepository,
            IRepository<Camino.Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> noiTruBenhAnRepository,
            IRepository<Core.Domain.Entities.KhamDoans.HopDongKhamSucKhoe> hopDongKhamSucKhoeRepository,
            IRepository<HopDongKhamSucKhoeNhanVien> hopDongKhamSucKhoeNhanVienRepository,
            IRepository<GiuongBenh> giuongBenhRepository,
            ICauHinhService cauHinhService,
            IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository, IRepository<VatTuBenhVien> vatTuBenhVienRepository,
            IRepository<Core.Domain.Entities.KhamDoans.CongTyKhamSucKhoe> congTyKhamSucKhoeRepository,
            IRepository<ICD> icdRepository,
            IRepository<PhauThuatThuThuatEkipBacSi> phauThuatThuThuatEkipBacSiRepository,
            IRepository<PhauThuatThuThuatEkipDieuDuong> phauThuatThuThuatEkipDieuDuongRepository,
            IRepository<YeuCauGoiDichVu> yeuCauGoiDichVuRepository,
            IRepository<Core.Domain.Entities.NoiGioiThieu.NoiGioiThieu> noiGioiThieuRepository,
            IRepository<BenhNhan> benhNhanRepository,
            IRepository<TaiKhoanBenhNhanChiThongTin> taiKhoanBenhNhanChiThongTinRepository,
            IRepository<HinhThucDen> hinhThucDenRepository,
            IRepository<DichVuKhamBenhBenhVien> dichVuKhamBenhBenhVienRepository,
            IRepository<NoiTruKhoaPhongDieuTri> noiTruKhoaPhongDieuTriRepository,
            IRepository<Core.Domain.Entities.MauVaChePhams.MauVaChePham> mauVaChePhamRepository,
            IRepository<DichVuBenhVienTongHop> dichVuBenhVienTongHopRepository,
            IRepository<YeuCauKhamBenhDonThuocChiTiet> yeuCauKhamBenhDonThuocChiTietRepository,
            IRepository<NoiTruPhieuDieuTri> noiTruPhieuDieuTriRepository,
            IRepository<YeuCauDichVuGiuongBenhVienChiPhiBHYT> yeuCauDichVuGiuongBenhVienChiPhiBhytRepository,
            IRepository<ChuongICD> chuongICDRepository,
            IRepository<NhomICDTheoBenhVien> nhomICDTheoBenhVienRepository,
            IRepository<YeuCauTraDuocPhamChiTiet> yeuCauTraDuocPhamChiTietRepository,
            IRepository<PhienXetNghiemChiTiet> phienXetNghiemChiTietRepository,
            IRepository<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTietRepository,
            IRepository<MayXetNghiem> mayXetNghiemRepository,
            IRepository<User> userRepository,
            IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
            IRepository<YeuCauTiepNhanDuLieuGuiCongBHYT> yeuCauTiepNhanDuLieuGuiCongBHYT,
            IRepository<ThuePhong> thuePhongRepository,
            IRepository<GoiKhamSucKhoeDichVuKhamBenh> goiKhamSucKhoeDichVuKhamBenhRepository,
            IRepository<GoiKhamSucKhoeDichVuDichVuKyThuat> goiKhamSucKhoeDichVuKyThuatRepository,
            IRepository<CongTyBaoHiemTuNhan> congTyBaoHiemTuNhanRepository,
            IRepository<NoiGioiThieuHopDong> noiGioiThieuHopDongRepository,
            IRepository<MienGiamChiPhi> mienGiamChiPhiRepository)
        {
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _nhanVienRepository = nhanVienRepository;
            _taiKhoanBenhNhanThuRepository = taiKhoanBenhNhanThuRepository;
            _taiKhoanBenhNhanChiRepository = taiKhoanBenhNhanChiRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _KhoaPhongRepository = KhoaPhongRepository;
            _templateRepository = templateRepository;
            _userAgentHelper = userAgentHelper;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _khoRepository = khoRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _xuatKhoDuocPhamChiTietViTriRepository = xuatKhoDuocPhamChiTietViTriRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository = yeuCauDichVuGiuongBenhVienChiPhiBenhVienRepository;
            _yeuCauDuocPhamBenhVienRepository = yeuCauDuocPhamBenhVienRepository;
            _yeuCauVatTuBenhVienRepository = yeuCauVatTuBenhVienRepository;
            _yeuCauTruyenMauRepository = yeuCauTruyenMauRepository;
            _donThuocThanhToanRepository = donThuocThanhToanRepository;
            _xuatKhoVatTuChiTietViTriRepository = xuatKhoVatTuChiTietViTriRepository;
            _dichVuXetNghiemKetNoiChiSoRepository = dichVuXetNghiemKetNoiChiSoRepository;
            _phienXetNghiemRepository = phienXetNghiemRepository;
            _noiTruBenhAnRepository = noiTruBenhAnRepository;
            _hopDongKhamSucKhoeRepository = hopDongKhamSucKhoeRepository;
            _hopDongKhamSucKhoeNhanVienRepository = hopDongKhamSucKhoeNhanVienRepository;
            _congTyKhamSucKhoeRepository = congTyKhamSucKhoeRepository;
            _giuongBenhRepository = giuongBenhRepository;
            _xuatKhoDuocPhamRepository = xuatKhoDuocPhamRepository;
            _cauHinhService = cauHinhService;
            _donThuocThanhToanChiTietRepository = donThuocThanhToanChiTietRepository;
            _xuatKhoVatTuRepository = xuatKhoVatTuRepository;
            _icdRepository = icdRepository;
            _donVTYTThanhToanChiTietRepository = donVTYTThanhToanChiTietRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _phauThuatThuThuatEkipBacSiRepository = phauThuatThuThuatEkipBacSiRepository;
            _phauThuatThuThuatEkipDieuDuongRepository = phauThuatThuThuatEkipDieuDuongRepository;
            _yeuCauGoiDichVuRepository = yeuCauGoiDichVuRepository;
            _noiGioiThieuRepository = noiGioiThieuRepository;
            _benhNhanRepository = benhNhanRepository;
            _yeuCauNhapKhoDuocPhamRepository = yeuCauNhapKhoDuocPhamRepository;
            _yeuCauNhapKhoVatTuRepository = yeuCauNhapKhoVatTuRepository;
            _duocPhamBenhVienPhanNhomRepository = duocPhamBenhVienPhanNhomRepository;
            _nhaThauRepository = nhaThauRepository;
            _hinhThucDenRepository = hinhThucDenRepository;
            _taiKhoanBenhNhanChiThongTinRepository = taiKhoanBenhNhanChiThongTinRepository;
            _dichVuKhamBenhBenhVienRepository = dichVuKhamBenhBenhVienRepository;
            _noiTruKhoaPhongDieuTriRepository = noiTruKhoaPhongDieuTriRepository;
            _mauVaChePhamRepository = mauVaChePhamRepository;
            _dichVuBenhVienTongHopRepository = dichVuBenhVienTongHopRepository;
            _noiTruPhieuDieuTriRepository = noiTruPhieuDieuTriRepository;
            _yeuCauKhamBenhDonThuocChiTietRepository = yeuCauKhamBenhDonThuocChiTietRepository;
            _yeuCauDichVuGiuongBenhVienChiPhiBhytRepository = yeuCauDichVuGiuongBenhVienChiPhiBhytRepository;
            _cauHinhRepository = cauHinhRepository;
            _chuongICDRepository = chuongICDRepository;
            _nhomICDTheoBenhVienRepository = nhomICDTheoBenhVienRepository;
            _yeuCauTraDuocPhamChiTietRepository = yeuCauTraDuocPhamChiTietRepository;
            _ketQuaXetNghiemChiTietRepository = ketQuaXetNghiemChiTietRepository;
            _phienXetNghiemChiTietRepository = phienXetNghiemChiTietRepository;
            _mayXetNghiemRepository = mayXetNghiemRepository;
            _userRepository = userRepository;
            _thuePhongRepository = thuePhongRepository;
            _yeuCauTiepNhanDuLieuGuiCongBHYT = yeuCauTiepNhanDuLieuGuiCongBHYT;
            _mienGiamChiPhiRepository = mienGiamChiPhiRepository;
            _goiKhamSucKhoeDichVuKhamBenhRepository = goiKhamSucKhoeDichVuKhamBenhRepository;
            _goiKhamSucKhoeDichVuKyThuatRepository = goiKhamSucKhoeDichVuKyThuatRepository;
            _congTyBaoHiemTuNhanRepository = congTyBaoHiemTuNhanRepository;
            _noiGioiThieuHopDongRepository = noiGioiThieuHopDongRepository;
            _yeuCauDichVuKyThuatKhamSangLocTiemChungRepository = yeuCauDichVuKyThuatKhamSangLocTiemChungRepository;
            _donThuocThanhToanChiTietTheoPhieuThuRepository = donThuocThanhToanChiTietTheoPhieuThuRepository;
            _donVTYTThanhToanChiTietTheoPhieuThuRepository = donVTYTThanhToanChiTietTheoPhieuThuRepository;
        }
    }
}
