using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKhamBenhs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKyThuats;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.FileKetQuaCanLamSangs;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.PhauThuatThuThuats;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.KhamBenhs;
using Camino.Services.KhoDuocPhams;
using Camino.Services.Localization;
using Camino.Services.TaiLieuDinhKem;

namespace Camino.Services.PhauThuatThuThuat
{
    [ScopedDependency(ServiceType = typeof(IPhauThuatThuThuatService))]
    public partial class PhauThuatThuThuatService : MasterFileService<YeuCauDichVuKyThuat>, IPhauThuatThuThuatService
    {
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVienHangDoi> _phongBenhVienHangDoiRepository;
        private readonly IRepository<Core.Domain.Entities.NhomChucDanhs.NhomChucDanh> _nhomChucDanhRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        //private readonly IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> _khoDuocPhamRepository;
        private readonly IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private readonly IRepository<PhauThuatThuThuatEkipBacSi> _phauThuatThuThuatEkipBacSiRepository;
        private readonly IRepository<PhauThuatThuThuatEkipDieuDuong> _phauThuatThuThuatEkipDieuDuongRepository;
        private readonly IRepository<TemplateKhamTheoDoi> _templateKhamTheoDoiRepository;
        private readonly IRepository<KhamTheoDoi> _khamTheoDoiRepository;
        private readonly IRepository<Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu> _ketQuaSinhHieuRepository;
        private readonly IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> _dichVuKyThuatBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> _nhomDichVuBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> _hoatDongNhanVienRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<KhamTheoDoiBoPhanKhac> _khamTheoDoiBoPhanKhacRepository;
        private readonly IRepository<YeuCauDichVuKyThuatLuocDoPhauThuat> _ycDvKtLdPtRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<TheoDoiSauPhauThuatThuThuat> _theoDoiSauPhauThuatThuThuatRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<Core.Domain.Entities.KetQuaNhomXetNghiems.KetQuaNhomXetNghiem> _ketQuaNhomXetNghiemsRepository;
        private IRepository<Kho> _khoRepository;
        private IRepository<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienRepository;
        private IRepository<YeuCauVatTuBenhVien> _yeuCauVatTuBenhVienRepository;
        private IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienService;
        private IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private IRepository<VatTuBenhVien> _vatTuBenhVienRepository;
        private IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        private IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        private IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private IRepository<XuatKhoDuocPham> _xuatKhoDuocPhamRepository;
        private IRepository<XuatKhoVatTu> _xuatKhoVatTuRepository;
        private IRepository<FileKetQuaCanLamSang> _fileKetQuaCanLamSanRepository;
        private readonly ITaiLieuDinhKemService _taiLieuDinhKemService;
        private ILocalizationService _localizationService;
        private ICauHinhService _cauHinhService;
        private readonly IRepository<PhienXetNghiem> _phienXetNghiemRepository;
        private readonly IRepository<PhienXetNghiemChiTiet> _phienXetNghiemChiTietRepository;
        private readonly IRepository<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTietRepository;
        private readonly IYeuCauDichVuKyThuatService _yeuCauDichVuKyThuatService;
        private readonly IRepository<YeuCauChayLaiXetNghiem> _yeuCauChayLaiXetNghiemRepository;
        private readonly IRepository<YeuCauGoiDichVu> _yeuCauGoiDichVuRepository;
        private readonly IRepository<YeuCauTraDuocPhamTuBenhNhanChiTiet> _yeuCauTraDuocPhamTuBenhNhanChiTietRepository;
        private readonly IRepository<YeuCauTraVatTuTuBenhNhanChiTiet> _yeuCauTraVatTuTuBenhNhanChiTietRepository;
        private IRepository<Core.Domain.Entities.HocViHocHams.HocViHocHam> _hocViHocHamRepository;
        private readonly IDuocPhamVaVatTuBenhVienService _duocPhamVaVatTuBenhVienService;
        private readonly IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> _khoNhanVienQuanLyRepository;
        private readonly IRepository<DuocPham> _duocPhamRepository;
        private readonly IRepository<Core.Domain.Entities.VatTus.VatTu> _vatTuRepository;
        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;

        private readonly IRepository<Core.Domain.Entities.ICDs.ICD> _iCDRepository;
        private readonly IRepository<ThuePhong> _thuePhongRepository;
        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinhThuePhong> _cauHinhThuePhongRepository;
        private readonly IRepository<DichVuKyThuatBenhVienNoiThucHienUuTien> _dichVuKyThuatBenhVienNoiThucHienUuTienRepository;
        private readonly IRepository<DichVuKyThuatBenhVienNoiThucHien> _dichVuKyThuatBenhVienNoiThucHienRepository;
        private readonly IRepository<YeuCauLinhDuocPhamChiTiet> _yeuCauLinhDuocPhamChiTietRepository;
        private readonly IRepository<YeuCauLinhVatTuChiTiet> _yeuCauLinhVatTuChiTietRepository;
        private readonly IRepository<NoiTruPhieuDieuTri> _noiTruPhieuDieuTriRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuDichVuKhamBenh> _chuongTrinhGoiDichVuKhamBenhRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuDichVuKyThuat> _chuongTrinhGoiDichVuKyThuatRepository;
        private readonly IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository;
        private readonly IRepository<MienGiamChiPhi> _mienGiamChiPhiRepository;
        private readonly IRepository<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChiRepository;

        public PhauThuatThuThuatService(
                IRepository<YeuCauDichVuKyThuat> repository,
                IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
                IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVienHangDoi> phongBenhVienHangDoiRepository,
                IRepository<Core.Domain.Entities.NhomChucDanhs.NhomChucDanh> nhomChucDanhRepository,
                IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
                //IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> khoDuocPhamRepository,
                IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository,
                IRepository<PhauThuatThuThuatEkipBacSi> phauThuatThuThuatEkipBacSiRepository,
                IRepository<PhauThuatThuThuatEkipDieuDuong> phauThuatThuThuatEkipDieuDuongRepository,
                IRepository<TemplateKhamTheoDoi> templateKhamTheoDoiRepository,
                IRepository<Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu> ketQuaSinhHieuRepository,
                IRepository<KhamTheoDoiBoPhanKhac> khamTheoDoiBoPhanKhacRepository,
                IRepository<KhamTheoDoi> khamTheoDoiRepository,
                IRepository<Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien> dichVuKyThuatBenhVienRepository,
                IRepository<Core.Domain.Entities.NhomDichVuBenhVien.NhomDichVuBenhVien> nhomDichVuBenhVienRepository,
                IUserAgentHelper userAgentHelper,
                IRepository<Core.Domain.Entities.NhanViens.HoatDongNhanVien> hoatDongNhanVienRepository,
                IRepository<YeuCauDichVuKyThuatLuocDoPhauThuat> ycDvKtLdPtRepository,
                IRepository<User> userRepository,
                IRepository<TheoDoiSauPhauThuatThuThuat> theoDoiSauPhauThuatThuThuatRepository,
                IRepository<Template> templateRepository,
                IRepository<Core.Domain.Entities.KetQuaNhomXetNghiems.KetQuaNhomXetNghiem> ketQuaNhomXetNghiemsRepository,
                IRepository<Kho> khoRepository,
                IRepository<YeuCauDuocPhamBenhVien> yeuCauDuocPhamBenhVienRepository,
                IRepository<YeuCauVatTuBenhVien> yeuCauVatTuBenhVienRepository,
                IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienService,
                IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository,
                IRepository<VatTuBenhVien> vatTuBenhVienRepository,
                IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository,
                IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository,
                IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository,
                IRepository<XuatKhoDuocPham> xuatKhoDuocPhamRepository,
                IRepository<XuatKhoVatTu> xuatKhoVatTuRepository,
                IRepository<FileKetQuaCanLamSang> fileKetQuaCanLamSanRepository,
                ITaiLieuDinhKemService taiLieuDinhKemService,
                ICauHinhService cauHinhService,
                ILocalizationService localizationService,
                IRepository<PhienXetNghiem> phienXetNghiemRepository,
                IRepository<PhienXetNghiemChiTiet> phienXetNghiemChiTietRepository,
                IRepository<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTietRepository,
                IYeuCauDichVuKyThuatService yeuCauDichVuKyThuatService,
                IRepository<YeuCauChayLaiXetNghiem> yeuCauChayLaiXetNghiemRepository,
                IRepository<YeuCauGoiDichVu> yeuCauGoiDichVuRepository,
                IRepository<YeuCauTraDuocPhamTuBenhNhanChiTiet> yeuCauTraDuocPhamTuBenhNhanChiTietRepository,
                IRepository<YeuCauTraVatTuTuBenhNhanChiTiet> yeuCauTraVatTuTuBenhNhanChiTietRepository,
                IRepository<Core.Domain.Entities.HocViHocHams.HocViHocHam> hocViHocHamRepository,
                IDuocPhamVaVatTuBenhVienService duocPhamVaVatTuBenhVienService,
                IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> khoNhanVienQuanLyRepository,
                IRepository<DuocPham> duocPhamRepository,
                IRepository<Core.Domain.Entities.VatTus.VatTu> vatTuRepository,
                IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
                IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
                IRepository<Core.Domain.Entities.ICDs.ICD> iCDRepository,
                IRepository<ThuePhong> thuePhongRepository,
                IRepository<Core.Domain.Entities.CauHinhs.CauHinhThuePhong> cauHinhThuePhongRepository,
                IRepository<DichVuKyThuatBenhVienNoiThucHienUuTien> dichVuKyThuatBenhVienNoiThucHienUuTienRepository,
                IRepository<DichVuKyThuatBenhVienNoiThucHien> dichVuKyThuatBenhVienNoiThucHienRepository,
                IRepository<YeuCauLinhDuocPhamChiTiet> yeuCauLinhDuocPhamChiTietRepository,
                IRepository<YeuCauLinhVatTuChiTiet> yeuCauLinhVatTuChiTietRepository,
                IRepository<NoiTruPhieuDieuTri> noiTruPhieuDieuTriRepository,
                IRepository<ChuongTrinhGoiDichVuDichVuKhamBenh> chuongTrinhGoiDichVuKhamBenhRepository,
                IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository,
                IRepository<ChuongTrinhGoiDichVuDichVuKyThuat> chuongTrinhGoiDichVuKyThuatRepository,
                IRepository<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository,
                IRepository<MienGiamChiPhi> mienGiamChiPhiRepository,
                IRepository<TaiKhoanBenhNhanChi> taiKhoanBenhNhanChiRepository
        ) : base(repository)
        {
            _phongBenhVienRepository = phongBenhVienRepository;
            _phongBenhVienHangDoiRepository = phongBenhVienHangDoiRepository;
            _nhomChucDanhRepository = nhomChucDanhRepository;
            _nhanVienRepository = nhanVienRepository;
            //_khoDuocPhamRepository = khoDuocPhamRepository;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _phauThuatThuThuatEkipDieuDuongRepository = phauThuatThuThuatEkipDieuDuongRepository;
            _phauThuatThuThuatEkipBacSiRepository = phauThuatThuThuatEkipBacSiRepository;
            _templateKhamTheoDoiRepository = templateKhamTheoDoiRepository;
            _khamTheoDoiRepository = khamTheoDoiRepository;
            _ketQuaSinhHieuRepository = ketQuaSinhHieuRepository;
            _dichVuKyThuatBenhVienRepository = dichVuKyThuatBenhVienRepository;
            _nhomDichVuBenhVienRepository = nhomDichVuBenhVienRepository;
            _userAgentHelper = userAgentHelper;
            _hoatDongNhanVienRepository = hoatDongNhanVienRepository;
            _khamTheoDoiBoPhanKhacRepository = khamTheoDoiBoPhanKhacRepository;
            _ycDvKtLdPtRepository = ycDvKtLdPtRepository;
            _userRepository = userRepository;
            _theoDoiSauPhauThuatThuThuatRepository = theoDoiSauPhauThuatThuThuatRepository;
            _templateRepository = templateRepository;
            _ketQuaNhomXetNghiemsRepository = ketQuaNhomXetNghiemsRepository;
            _khoRepository = khoRepository;
            _yeuCauDuocPhamBenhVienRepository = yeuCauDuocPhamBenhVienRepository;
            _yeuCauVatTuBenhVienRepository = yeuCauVatTuBenhVienRepository;
            _duocPhamBenhVienService = duocPhamBenhVienService;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _xuatKhoDuocPhamRepository = xuatKhoDuocPhamRepository;
            _xuatKhoVatTuRepository = xuatKhoVatTuRepository;
            _fileKetQuaCanLamSanRepository = fileKetQuaCanLamSanRepository;
            _taiLieuDinhKemService = taiLieuDinhKemService;
            _localizationService = localizationService;
            _cauHinhService = cauHinhService;
            _phienXetNghiemRepository = phienXetNghiemRepository;
            _phienXetNghiemChiTietRepository = phienXetNghiemChiTietRepository;
            _ketQuaXetNghiemChiTietRepository = ketQuaXetNghiemChiTietRepository;
            _yeuCauDichVuKyThuatService = yeuCauDichVuKyThuatService;
            _yeuCauChayLaiXetNghiemRepository = yeuCauChayLaiXetNghiemRepository;
            _yeuCauGoiDichVuRepository = yeuCauGoiDichVuRepository;
            _yeuCauTraDuocPhamTuBenhNhanChiTietRepository = yeuCauTraDuocPhamTuBenhNhanChiTietRepository;
            _yeuCauTraVatTuTuBenhNhanChiTietRepository = yeuCauTraVatTuTuBenhNhanChiTietRepository;
            _hocViHocHamRepository = hocViHocHamRepository;
            _duocPhamVaVatTuBenhVienService = duocPhamVaVatTuBenhVienService;
            _khoNhanVienQuanLyRepository = khoNhanVienQuanLyRepository;
            _duocPhamRepository = duocPhamRepository;
            _vatTuRepository = vatTuRepository;
            _cauHinhRepository = cauHinhRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _iCDRepository = iCDRepository;
            _thuePhongRepository = thuePhongRepository;
            _cauHinhThuePhongRepository = cauHinhThuePhongRepository;
            _dichVuKyThuatBenhVienNoiThucHienUuTienRepository = dichVuKyThuatBenhVienNoiThucHienUuTienRepository;
            _dichVuKyThuatBenhVienNoiThucHienRepository = dichVuKyThuatBenhVienNoiThucHienRepository;
            _yeuCauLinhDuocPhamChiTietRepository = yeuCauLinhDuocPhamChiTietRepository;
            _yeuCauLinhVatTuChiTietRepository = yeuCauLinhVatTuChiTietRepository;
            _noiTruPhieuDieuTriRepository = noiTruPhieuDieuTriRepository;
            _chuongTrinhGoiDichVuKhamBenhRepository = chuongTrinhGoiDichVuKhamBenhRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhRepository;
            _chuongTrinhGoiDichVuKyThuatRepository = chuongTrinhGoiDichVuKyThuatRepository;
            _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository = chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuatRepository;
            _mienGiamChiPhiRepository = mienGiamChiPhiRepository;
            _taiKhoanBenhNhanChiRepository = taiKhoanBenhNhanChiRepository;
        }
    }
}
