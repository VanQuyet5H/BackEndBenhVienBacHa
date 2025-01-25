using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhams;
using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.Localization;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.Users;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Services.YeuCauKhamBenh;

namespace Camino.Services.YeuCauMuaDuTruDuocPham
{
    [ScopedDependency(ServiceType = typeof(IYeuCauMuaDuTruDuocPhamService))]
    public partial class YeuCauMuaDuTruDuocPhamService : MasterFileService<DuTruMuaDuocPham>, IYeuCauMuaDuTruDuocPhamService
    {

        private readonly IRepository<Template> _templateRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<LocaleStringResource> _localeStringResourceRepository;
        private readonly IRepository<KhoNhanVienQuanLy> _khoNhanVienQuanLyRepository;
        private readonly IRepository<KyDuTruMuaDuocPhamVatTu> _kyDuTruMuaDuocPhamVatTuRepository;
        private readonly IRepository<DuocPham> _duocPhamRepository;
        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        private readonly IRepository<NhapKhoDuocPham> _nhapKhoDuocPhamRepository;
        private readonly IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private readonly IRepository<Kho> _khoRepository;
        private readonly IRepository<HopDongThauDuocPham> _hopDongThauDuocPhamRepository;
        private readonly IRepository<HopDongThauDuocPhamChiTiet> _hopDongThauDuocPhamChiTietRepository;
        private readonly IRepository<DuTruMuaDuocPhamChiTiet> _duTruMuaDuocPhamChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<User> _useRepository;
        private readonly IRepository<DuTruMuaDuocPhamChiTiet> _duTruMuaDuocPhamChiTietRepo;
        private readonly IRepository<DuTruMuaDuocPhamKhoDuoc> _duTruMuaDuocPhamKhoDuocRepo;
        private readonly IRepository<DuTruMuaDuocPhamKhoDuocChiTiet> _duTruMuaDuocPhamKhoDuocChiTietRepo;
        //private readonly IRepository<Core.Domain.Entities.NhapKhoDuocPhamChiTiets.NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private readonly IRepository<HopDongThauDuocPhamChiTiet> _hopDongThauDuocPhamChiTiet;
        private readonly IRepository<DuTruMuaDuocPhamTheoKhoaChiTiet> _duTruMuaDuocPhamTheoKhoaChiTietRepo;
        private readonly IRepository<DuTruMuaDuocPhamTheoKhoa> _duTruMuaDuocPhamTheoKhoaRepo;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepo;
        IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;

        private readonly IRepository<DuTruMuaDuocPhamTheoKhoa> _duTruMuaDuocPhamTheoKhoaRepository;
        private readonly IRepository<DuTruMuaDuocPhamTheoKhoaChiTiet> _duTruMuaDuocPhamTheoKhoaChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> _khoRepo;

        public YeuCauMuaDuTruDuocPhamService(
            IRepository<DuTruMuaDuocPham> repository,
            IRepository<Template> templateRepository,
            ILocalizationService localizationService,
            IUserAgentHelper userAgentHelper,
            IRepository<User> userRepository,
            ICauHinhService cauHinhService,
            IRepository<LocaleStringResource> localeStringResourceRepository,
            IRepository<KhoNhanVienQuanLy> khoNhanVienQuanLyRepository,
            IRepository<KyDuTruMuaDuocPhamVatTu> kyDuTruMuaDuocPhamVatTuRepository,
            IRepository<DuocPham> duocPhamRepository,
            IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository,
            IRepository<NhapKhoDuocPham> nhapKhoDuocPhamRepository,
            IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository,
            IRepository<Kho> khoRepository,
            IRepository<HopDongThauDuocPham> hopDongThauDuocPhamRepository,
            IRepository<HopDongThauDuocPhamChiTiet> hopDongThauDuocPhamChiTietRepository,
            IRepository<DuTruMuaDuocPhamChiTiet> duTruMuaDuocPhamChiTietRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
            IRepository<User> useRepository,
            IRepository<DuTruMuaDuocPhamTheoKhoa> duTruMuaDuocPhamTheoKhoaRepository,
            IRepository<DuTruMuaDuocPhamTheoKhoaChiTiet> duTruMuaDuocPhamTheoKhoaChiTietRepository,
            //IRepository<LocaleStringResource> localeStringResourceRepository,
            IRepository<DuTruMuaDuocPhamChiTiet> duTruMuaDuocPhamChiTietRepo,
            //IRepository<Core.Domain.Entities.NhapKhoDuocPhamChiTiets.NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository,
            IRepository<HopDongThauDuocPhamChiTiet> hopDongThauDuocPhamChiTiet,
            IRepository<DuTruMuaDuocPhamTheoKhoaChiTiet> duTruMuaDuocPhamTheoKhoaChiTietRepo,
            IRepository<DuTruMuaDuocPhamTheoKhoa> duTruMuaDuocPhamTheoKhoaRepo,
            IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepo,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<DuTruMuaDuocPhamKhoDuoc> duTruMuaDuocPhamKhoDuocRepo,
            IRepository<DuTruMuaDuocPhamKhoDuocChiTiet> duTruMuaDuocPhamKhoDuocChiTietRepo,
            IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> khoRepo
            ) : base(repository)
        {
            _templateRepository = templateRepository;
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
            _userRepository = userRepository;
            _cauHinhService = cauHinhService;
            _localeStringResourceRepository = localeStringResourceRepository;
            _khoNhanVienQuanLyRepository = khoNhanVienQuanLyRepository;
            _kyDuTruMuaDuocPhamVatTuRepository = kyDuTruMuaDuocPhamVatTuRepository;
            _duocPhamRepository = duocPhamRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _nhapKhoDuocPhamRepository = nhapKhoDuocPhamRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _khoRepository = khoRepository;
            _hopDongThauDuocPhamRepository = hopDongThauDuocPhamRepository;
            _hopDongThauDuocPhamChiTietRepository = hopDongThauDuocPhamChiTietRepository;
            _duTruMuaDuocPhamChiTietRepository = duTruMuaDuocPhamChiTietRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _useRepository = useRepository;
            _duTruMuaDuocPhamTheoKhoaRepository = duTruMuaDuocPhamTheoKhoaRepository;
            _duTruMuaDuocPhamTheoKhoaChiTietRepository = duTruMuaDuocPhamTheoKhoaChiTietRepository;
            _duTruMuaDuocPhamChiTietRepo = duTruMuaDuocPhamChiTietRepo;
            //_nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _hopDongThauDuocPhamChiTiet = hopDongThauDuocPhamChiTiet;
            _duTruMuaDuocPhamTheoKhoaChiTietRepo = duTruMuaDuocPhamTheoKhoaChiTietRepo;
            _duTruMuaDuocPhamTheoKhoaRepo = duTruMuaDuocPhamTheoKhoaRepo;
            _khoaPhongRepo = khoaPhongRepo;
            _nhanVienRepository = nhanVienRepository;
            _duTruMuaDuocPhamKhoDuocRepo = duTruMuaDuocPhamKhoDuocRepo;
            _duTruMuaDuocPhamKhoDuocChiTietRepo = duTruMuaDuocPhamKhoDuocChiTietRepo;
            _khoRepo = khoRepo;
        }
    }
}
