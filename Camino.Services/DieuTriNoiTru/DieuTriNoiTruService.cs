using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.Localization;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.YeuCauNhapViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhanCongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhanLichSuChuyenDoiTuongs;
using Camino.Services.BenhNhans;
using Camino.Services.YeuCauTiepNhans;
using Camino.Services.KhamBenhs;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.NoiTruDonThuocs;
using Camino.Services.YeuCauKhamBenh;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Services.ToaThuocMau;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Camino.Core.Domain.Entities.XuatKhos;

namespace Camino.Services.DieuTriNoiTru
{
    [ScopedDependency(ServiceType = typeof(IDieuTriNoiTruService))]
    public partial class DieuTriNoiTruService : YeuCauTiepNhanBaseService, IDieuTriNoiTruService 
    {
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> _khoRepository;
        private readonly IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> _khoNhanVienQuanLyRepository;
        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly IRepository<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienRepository;
        private readonly IRepository<ADR> _aDRRepository;
        private readonly IRepository<DuocPham> _duocPhamRepository;
        private readonly IRepository<NhapKhoDuocPham> _nhapKhoDuocPhamRepository;
        private readonly IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private readonly IRepository<ThuocHoacHoatChat> _thuocHoacHoatChatRepository;
        private readonly IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> _noiTruBenhAnRepository;
        private readonly IRepository<NoiTruKhoaPhongDieuTri> _noiTruKhoaPhongDieuTriRepository;
        private readonly IRepository<NoiTruEkipDieuTri> _noiTruEkipDieuTriRepository;
        private readonly IRepository<ICD> _icdRepository;
        private readonly IRepository<LocaleStringResource> _localeStringResourceRepository;
        private readonly IRepository<NoiTruPhieuDieuTri> _noiTruPhieuDieuTriRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> _dichVuGiuongBenhVienRepository;
        private readonly IRepository<GiuongBenh> _giuongBenhRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<YeuCauVatTuBenhVien> _yeuCauVatTuBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.VatTus.VatTu> _vatTuRepository;
        private readonly IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        private readonly IRepository<NhapKhoVatTu> _nhapKhoVatTuRepository;
        private readonly IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private readonly IRepository<NoiTruHoSoKhac> _noiTruHoSoKhacRepository;
        private readonly IRepository<NoiTruHoSoKhacFileDinhKem> _noiTruHoSoKhacFileDinhKemRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<Camino.Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> _khoaPhongNhanVienResourceRepository;
        private readonly IRepository<Core.Domain.Entities.BenhVien.BenhVien> _benhVienRepository;
        private readonly IRepository<YeuCauTruyenMau> _yeuCauTruyenMauRepository;
        private readonly IRepository<Core.Domain.Entities.MauVaChePhams.MauVaChePham> _mauVaChePhamRepository;
        private readonly IRepository<NhapKhoMauChiTiet> _nhapKhoMauChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.MauVaChePhams.NhapKhoMau> _nhapKhoMauRepository;
        private readonly IRepository<Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan> _quanHeThanNhanRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> _hoatDongNhanVienRepository;
        private readonly IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        private readonly IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private readonly IRepository<Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu> _ketQuaSinhHieuRepository;
        private readonly IRepository<NoiTruThamKhamChanDoanKemTheo> _noiTruThamKhamChanDoanKemTheoRepository;
        private readonly IRepository<BenhNhanDiUngThuoc> _BenhNhanDiUngThuocRepository;
        private readonly IRepository<User> _useRepository;
        private readonly IRepository<NoiTruChiDinhDuocPham> _noiTruChiDinhDuocPhamRepository;
        private readonly IRepository<YeuCauTraDuocPhamTuBenhNhanChiTiet> _yeuCauTraDuocPhamTuBenhNhanChiTietRepository;
        private readonly IRepository<YeuCauTraVatTuTuBenhNhanChiTiet> _yeuCauTraVatTuTuBenhNhanChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan> _yeuCauTraVatTuTuBenhNhanRepository;
        private readonly IRepository<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.ChucDanhs.ChucDanh> _chucDanhRepository;
        private readonly IRepository<Core.Domain.Entities.VanBangChuyenMons.VanBangChuyenMon> _vanBangChuyenMonRepository;
        private readonly IRepository<Core.Domain.Entities.NgheNghieps.NgheNghiep> _ngheNghiepRepository;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        private readonly IRepository<Camino.Core.Domain.Entities.BenhVien.Khoas.Khoa> _khoaRepository;
        private readonly IDanhSachChoKhamService _danhSachChoKhamService;

        private readonly IRepository<YeuCauTiepNhanLichSuChuyenDoiTuong> _yeuCauTiepNhanLichSuChuyenDoiTuongRepository;
        private readonly IRepository<CongTyBaoHiemTuNhan> _congTyBaoHiemTuNhanRepository;
        private readonly IRepository<YeuCauNhapVien> _yeuCauNhapVienRepository;
        private readonly IRepository<Core.Domain.Entities.DanTocs.DanToc> _danTocRepository;
        private readonly IRepository<Core.Domain.Entities.QuocGias.QuocGia> _quocGiaRepository;
        private readonly IRepository<NoiTruPhieuDieuTriTuVanThuoc> _noiTruPhieuDieuTriTuVanThuocRepository;
        private readonly IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly IRepository<HoatDongGiuongBenh> _hoatDongGiuongBenhRepository;
        private readonly IRepository<YeuCauGoiDichVu> _yeuCauGoiDichVuRepository;
        private readonly IKhamBenhService _khamBenhService;
        private readonly IRepository<PhienXetNghiem> _phienXetNghiemRepository;
        private readonly IRepository<PhienXetNghiemChiTiet> _phienXetNghiemChiTietRepository;
        private readonly IRepository<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IRepository<NoiTruDonThuoc> _noiTruDonThuocRepository;
        private readonly IRepository<NoiTruDonThuocChiTiet> _noiTruDonThuocChiTietRepository;
        private readonly IRepository<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.CheDoAns.CheDoAn> _cheDoAnRepository;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;

        IRepository<DuocPhamBenhVienPhanNhom> _duocPhamBenhVienPhanNhomRepository;

        private readonly IRepository<Core.Domain.Entities.NoiDungMauLoiDanBacSi.NoiDungMauLoiDanBacSi> _noiDungMauLoiDanBacSiRepository;

        private IRepository<DuocPhamVaVatTuBenhVien> _duocPhamVaVatTuBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.InputStringStoreds.InputStringStored> _inputStringStoredRepository;

        private readonly IRepository<Core.Domain.Entities.TinhTrangRaVienHoSoKhacs.TinhTrangRaVienHoSoKhac> _tinhTrangRaVienHoSoKhacRepository;
        private readonly IRepository<Core.Domain.Entities.BenhNhans.BenhNhanDiUngThuoc> _benhNhanDiUngThuocRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> _dichVuKyThuatRepository;

        private readonly IRepository<PhauThuatThuThuatEkipBacSi> _phauThuatThuThuatEkipBacSiRepository;
        private readonly IRepository<PhauThuatThuThuatEkipDieuDuong> _phauThuatThuThuatEkipDieuDuongRepository;
        private readonly IRepository<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTriRepository;
        private readonly IRepository<MienGiamChiPhi> _mienGiamChiPhiRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhICDKhac> _yeuCauKhamBenhICDKhacRepository;
        private readonly IRepository<NoiTruPhieuDieuTriChiTietYLenh> _noiTruPhieuDieuTriChiTietYLenhRepository;

        public DieuTriNoiTruService(
            IRepository<YeuCauTiepNhan> repository, //chỉnh lại
            IUserAgentHelper userAgentHelper,
            IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> khoRepository,
            IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> khoNhanVienQuanLyRepository,
            IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository,
            ICauHinhService cauHinhService,
            IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository,
            IRepository<YeuCauDuocPhamBenhVien> yeuCauDuocPhamBenhVienRepository,
            IRepository<ADR> aDRRepository,
            IRepository<DuocPham> duocPhamRepository,
            IRepository<NhapKhoDuocPham> nhapKhoDuocPhamRepository,
            IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository,
            IRepository<ThuocHoacHoatChat> thuocHoacHoatChatRepository,
            IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruBenhAn> noiTruBenhAnRepository,
            IRepository<NoiTruKhoaPhongDieuTri> noiTruKhoaPhongDieuTriRepository,
            IRepository<NoiTruEkipDieuTri> noiTruEkipDieuTriRepository,
            IRepository<LocaleStringResource> localeStringResourceRepository,
            IRepository<NoiTruPhieuDieuTri> noiTruPhieuDieuTriRepository,
            IRepository<ICD> icdRepository,
            ILocalizationService localizationService,
            ITaiKhoanBenhNhanService taiKhoanBenhNhanService,
            IRepository<Core.Domain.Entities.DichVuGiuongBenhViens.DichVuGiuongBenhVien> dichVuGiuongBenhVienRepository,
            IRepository<GiuongBenh> giuongBenhRepository,
            IRepository<YeuCauVatTuBenhVien> yeuCauVatTuBenhVienRepository,
            IRepository<Core.Domain.Entities.VatTus.VatTu> vatTuRepository,
            IRepository<VatTuBenhVien> vatTuBenhVienRepository,
            IRepository<NhapKhoVatTu> nhapKhoVatTuRepository,
            IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<Core.Domain.Entities.DieuTriNoiTrus.NoiTruHoSoKhac> noiTruHoSoKhacRepository,
            IRepository<Template> templateRepository,
            IRepository<Camino.Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> khoaPhongNhanVienResourceRepository,
            IRepository<Core.Domain.Entities.BenhVien.BenhVien> benhVienRepository,
            IRepository<YeuCauTruyenMau> yeuCauTruyenMauRepository,
            IRepository<Core.Domain.Entities.MauVaChePhams.MauVaChePham> mauVaChePhamRepository,
            IRepository<NhapKhoMauChiTiet> nhapKhoMauChiTietRepository,
            IRepository<Core.Domain.Entities.MauVaChePhams.NhapKhoMau> nhapKhoMauRepository,
            IRepository<Core.Domain.Entities.QuanHeThanNhans.QuanHeThanNhan> quanHeThanNhanRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
            IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> hoatDongNhanVienRepository,
            IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
            IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository,
            IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
            IRepository<Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu> ketQuaSinhHieuRepository,
            IRepository<NoiTruHoSoKhacFileDinhKem> noiTruHoSoKhacFileDinhKemRepository,
            IRepository<NoiTruThamKhamChanDoanKemTheo> noiTruThamKhamChanDoanKemTheoRepository,
            IRepository<BenhNhanDiUngThuoc> BenhNhanDiUngThuocRepository,
            IRepository<User> useRepository,
            IRepository<NoiTruChiDinhDuocPham> noiTruChiDinhDuocPhamRepository,
            IRepository<YeuCauTraDuocPhamTuBenhNhanChiTiet> yeuCauTraDuocPhamTuBenhNhanChiTietRepository,
            IRepository<YeuCauTraVatTuTuBenhNhanChiTiet> yeuCauTraVatTuTuBenhNhanChiTietRepository,
            IRepository<Core.Domain.Entities.ChucDanhs.ChucDanh> chucDanhRepository,
            IRepository<Core.Domain.Entities.VanBangChuyenMons.VanBangChuyenMon> vanBangChuyenMonRepository,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan> yeuCauTraVatTuTuBenhNhanRepository,
            IRepository<YeuCauDichVuGiuongBenhVien> yeuCauDichVuGiuongBenhVienRepository,
            IRepository<Core.Domain.Entities.NgheNghieps.NgheNghiep> ngheNghiepRepository,
            IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository,
            IDanhSachChoKhamService danhSachChoKhamService,
            IRepository<YeuCauTiepNhanLichSuChuyenDoiTuong> yeuCauTiepNhanLichSuChuyenDoiTuongRepository,
            IRepository<CongTyBaoHiemTuNhan> congTyBaoHiemTuNhanRepository,
            IRepository<YeuCauNhapVien> yeuCauNhapVienRepository,
            IRepository<Core.Domain.Entities.DanTocs.DanToc> danTocRepository,
            IRepository<Core.Domain.Entities.QuocGias.QuocGia> quocGiaRepository,
            IRepository<NoiTruPhieuDieuTriTuVanThuoc> noiTruPhieuDieuTriTuVanThuocRepository,
            IRepository<HoatDongGiuongBenh> hoatDongGiuongBenhRepository,
            IRepository<YeuCauGoiDichVu> yeuCauGoiDichVuRepository,
            IKhamBenhService khamBenhService,
            IRepository<PhienXetNghiem> phienXetNghiemRepository,
            IRepository<PhienXetNghiemChiTiet> phienXetNghiemChiTietRepository,
            IRepository<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTietRepository,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
            IRepository<NoiTruDonThuoc> noiTruDonThuocRepository,
            IRepository<NoiTruDonThuocChiTiet> noiTruDonThuocChiTietRepository,
            IRepository<Core.Domain.Entities.CheDoAns.CheDoAn> cheDoAnRepository,
            IYeuCauKhamBenhService yeuCauKhamBenhService, 
            IRepository<DuocPhamBenhVienPhanNhom> duocPhamBenhVienPhanNhomRepository,
            IRepository<Core.Domain.Entities.NoiDungMauLoiDanBacSi.NoiDungMauLoiDanBacSi> noiDungMauLoiDanBacSiRepository,
            IRepository<Core.Domain.Entities.InputStringStoreds.InputStringStored> inputStringStoredRepository,
            IRepository<Core.Domain.Entities.TinhTrangRaVienHoSoKhacs.TinhTrangRaVienHoSoKhac> tinhTrangRaVienHoSoKhacRepository,
            IRepository<Core.Domain.Entities.BenhNhans.BenhNhanDiUngThuoc> benhNhanDiUngThuocRepository,
            IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuat> dichVuKyThuatRepository,
            IRepository<PhauThuatThuThuatEkipBacSi> phauThuatThuThuatEkipBacSiRepository,
            IRepository<PhauThuatThuThuatEkipDieuDuong> phauThuatThuThuatEkipDieuDuongRepository,
            IRepository<DonThuocThanhToanChiTiet> donThuocThanhToanChiTietRepository,
            IRepository<XuatKhoDuocPhamChiTietViTri> xuatKhoDuocPhamChiTietViTriRepository,
            IRepository<Camino.Core.Domain.Entities.BenhVien.Khoas.Khoa> khoaRepository,
            IRepository<MienGiamChiPhi> mienGiamChiPhiRepository,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhICDKhac> yeuCauKhamBenhICDKhacRepository,
            IRepository<NoiTruPhieuDieuTriChiTietYLenh> noiTruPhieuDieuTriChiTietYLenhRepository
        ) : base(repository, userAgentHelper, cauHinhService, localizationService, taiKhoanBenhNhanService)
        {
            _userAgentHelper = userAgentHelper;
            _khoRepository = khoRepository;
            _khoNhanVienQuanLyRepository = khoNhanVienQuanLyRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _cauHinhService = cauHinhService;
            _yeuCauDuocPhamBenhVienRepository = yeuCauDuocPhamBenhVienRepository;
            _aDRRepository = aDRRepository;
            _duocPhamRepository = duocPhamRepository;
            _nhapKhoDuocPhamRepository = nhapKhoDuocPhamRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _thuocHoacHoatChatRepository = thuocHoacHoatChatRepository;
            _noiTruBenhAnRepository = noiTruBenhAnRepository;
            _noiTruKhoaPhongDieuTriRepository = noiTruKhoaPhongDieuTriRepository;
            _noiTruEkipDieuTriRepository = noiTruEkipDieuTriRepository;
            _icdRepository = icdRepository;
            _localeStringResourceRepository = localeStringResourceRepository;
            _noiTruPhieuDieuTriRepository = noiTruPhieuDieuTriRepository;
            _localizationService = localizationService;
            _dichVuGiuongBenhVienRepository = dichVuGiuongBenhVienRepository;
            _giuongBenhRepository = giuongBenhRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _yeuCauVatTuBenhVienRepository = yeuCauVatTuBenhVienRepository;
            _vatTuRepository = vatTuRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _nhapKhoVatTuRepository = nhapKhoVatTuRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _nhanVienRepository = nhanVienRepository;
            _noiTruHoSoKhacRepository = noiTruHoSoKhacRepository;
            _templateRepository = templateRepository;
            _khoaPhongNhanVienResourceRepository = khoaPhongNhanVienResourceRepository;
            _benhVienRepository = benhVienRepository;
            _yeuCauTruyenMauRepository = yeuCauTruyenMauRepository;
            _mauVaChePhamRepository = mauVaChePhamRepository;
            _nhapKhoMauChiTietRepository = nhapKhoMauChiTietRepository;
            _nhapKhoMauRepository = nhapKhoMauRepository;
            _quanHeThanNhanRepository = quanHeThanNhanRepository;
            _hoatDongNhanVienRepository = hoatDongNhanVienRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _cauHinhRepository = cauHinhRepository;
            _ketQuaSinhHieuRepository = ketQuaSinhHieuRepository;
            _noiTruHoSoKhacFileDinhKemRepository = noiTruHoSoKhacFileDinhKemRepository;
            _noiTruThamKhamChanDoanKemTheoRepository = noiTruThamKhamChanDoanKemTheoRepository;
            _BenhNhanDiUngThuocRepository = BenhNhanDiUngThuocRepository;
            _useRepository = useRepository;
            _noiTruChiDinhDuocPhamRepository = noiTruChiDinhDuocPhamRepository;
            _yeuCauTraDuocPhamTuBenhNhanChiTietRepository = yeuCauTraDuocPhamTuBenhNhanChiTietRepository;
            _yeuCauTraVatTuTuBenhNhanChiTietRepository = yeuCauTraVatTuTuBenhNhanChiTietRepository;
            _yeuCauTraVatTuTuBenhNhanRepository = yeuCauTraVatTuTuBenhNhanRepository;
            _yeuCauDichVuGiuongBenhVienRepository = yeuCauDichVuGiuongBenhVienRepository;
            _chucDanhRepository = chucDanhRepository;
            _vanBangChuyenMonRepository = vanBangChuyenMonRepository;
            _ngheNghiepRepository = ngheNghiepRepository;
            _khoaPhongRepository = khoaPhongRepository;
            _danhSachChoKhamService = danhSachChoKhamService;
            _yeuCauTiepNhanLichSuChuyenDoiTuongRepository = yeuCauTiepNhanLichSuChuyenDoiTuongRepository;
            _congTyBaoHiemTuNhanRepository = congTyBaoHiemTuNhanRepository;
            _yeuCauNhapVienRepository = yeuCauNhapVienRepository;
            _danTocRepository = danTocRepository;
            _quocGiaRepository = quocGiaRepository;
            _noiTruPhieuDieuTriTuVanThuocRepository = noiTruPhieuDieuTriTuVanThuocRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _hoatDongGiuongBenhRepository = hoatDongGiuongBenhRepository;
            _yeuCauGoiDichVuRepository = yeuCauGoiDichVuRepository;
            _khamBenhService = khamBenhService;
            _phienXetNghiemRepository = phienXetNghiemRepository;
            _phienXetNghiemChiTietRepository = phienXetNghiemChiTietRepository;
            _ketQuaXetNghiemChiTietRepository = ketQuaXetNghiemChiTietRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _noiTruDonThuocRepository = noiTruDonThuocRepository;
            _noiTruDonThuocChiTietRepository = noiTruDonThuocChiTietRepository;
            _cheDoAnRepository = cheDoAnRepository;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _duocPhamBenhVienPhanNhomRepository = duocPhamBenhVienPhanNhomRepository;
            _noiDungMauLoiDanBacSiRepository = noiDungMauLoiDanBacSiRepository;
            _inputStringStoredRepository = inputStringStoredRepository;
            _tinhTrangRaVienHoSoKhacRepository = tinhTrangRaVienHoSoKhacRepository;
            _benhNhanDiUngThuocRepository = benhNhanDiUngThuocRepository;
            _dichVuKyThuatRepository = dichVuKyThuatRepository;
            _phauThuatThuThuatEkipBacSiRepository = phauThuatThuThuatEkipBacSiRepository;
            _phauThuatThuThuatEkipDieuDuongRepository = phauThuatThuThuatEkipDieuDuongRepository;
            _xuatKhoDuocPhamChiTietViTriRepository = xuatKhoDuocPhamChiTietViTriRepository;
            _donThuocThanhToanChiTietRepository = donThuocThanhToanChiTietRepository;
            _khoaRepository = khoaRepository;
            _mienGiamChiPhiRepository = mienGiamChiPhiRepository;
            _yeuCauKhamBenhICDKhacRepository = yeuCauKhamBenhICDKhacRepository;
            _noiTruPhieuDieuTriChiTietYLenhRepository = noiTruPhieuDieuTriChiTietYLenhRepository;
        }
    }
}
