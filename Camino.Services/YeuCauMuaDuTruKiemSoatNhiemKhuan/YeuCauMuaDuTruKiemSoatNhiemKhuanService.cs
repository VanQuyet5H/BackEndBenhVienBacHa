using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhoNhanVienQuanLys;
using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Camino.Core.Domain.Entities.Localization;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.Users;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Core.Domain.Entities.DuTruVatTus;
using Camino.Core.Domain.Entities.KyDuTruMuaDuocPhamVatTus;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;

namespace Camino.Services.YeuCauMuaDuTruKiemSoatNhiemKhuan
{
    [ScopedDependency(ServiceType = typeof(IYeuCauMuaDuTruKiemSoatNhiemKhuanService))]

    public partial class YeuCauMuaDuTruKiemSoatNhiemKhuanService : MasterFileService<DuTruMuaVatTu>, IYeuCauMuaDuTruKiemSoatNhiemKhuanService
    {

        private readonly IRepository<Template> _templateRepository;
        private readonly ICauHinhService _cauHinhService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<LocaleStringResource> _localeStringResourceRepository;
        private readonly IRepository<KhoNhanVienQuanLy> _khoNhanVienQuanLyRepository;
        private readonly IRepository<KyDuTruMuaDuocPhamVatTu> _kyDuTruMuaDuocPhamVatTuRepository;
        private readonly IRepository<Core.Domain.Entities.VatTus.VatTu> _vatTuRepository;
        private readonly IRepository<Core.Domain.Entities.VatTuBenhViens.VatTuBenhVien> _vatTuBenhVienRepository;
        private readonly IRepository<NhapKhoVatTu> _nhapKhoVatTuRepository;
        private readonly IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        private readonly IRepository<Kho> _khoRepository;
        private readonly IRepository<HopDongThauVatTu> _hopDongThauVatTuRepository;
        private readonly IRepository<HopDongThauVatTuChiTiet> _hopDongThauVatTuChiTietRepository;
        private readonly IRepository<DuTruMuaVatTuChiTiet> _duTruMuaVatTuChiTietRepository;
        private readonly IRepository<DuTruMuaVatTuTheoKhoa> _duTruMuaVatTuTheoKhoaRepository;
        private readonly IRepository<DuTruMuaVatTuTheoKhoaChiTiet> _duTruMuaVatTuTheoKhoaChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;        
        private readonly IRepository<DuTruMuaVatTuKhoDuoc> _duTruMuaVatTuKhoDuocRepository;
        private readonly IRepository<DuTruMuaVatTuKhoDuocChiTiet> _duTruMuaVatTuKhoDuocChiTietRepository;  
        private readonly IRepository<Core.Domain.Entities.NhomVatTus.NhomVatTu> _nhomVatTuRepository;


        private readonly IRepository<Core.Domain.Entities.DuTruMuaDuocPhams.DuTruMuaDuocPham> _duTruMuaDuocPhamRepository;
        private readonly IRepository<Core.Domain.Entities.DuTruMuaDuocPhams.DuTruMuaDuocPhamChiTiet> _duTruMuaDuocPhamChiTietRepository;

        private readonly IRepository<DuTruMuaDuocPhamTheoKhoa> _duTruMuaDuocPhamTheoKhoaRepository;
        private readonly IRepository<DuTruMuaDuocPhamTheoKhoaChiTiet> _duTruMuaDuocPhamTheoKhoaChiTietRepository;
        private readonly IRepository<DuTruMuaDuocPhamKhoDuocChiTiet> _duTruMuaDuocPhamKhoDuocChiTietRepository;
        private readonly IRepository<DuTruMuaDuocPhamKhoDuoc> _duTruMuaDuocPhamKhoDuocRepo;
        public YeuCauMuaDuTruKiemSoatNhiemKhuanService(
            IRepository<DuTruMuaVatTu> repository,
            IRepository<Template> templateRepository,
            ILocalizationService localizationService,
            IUserAgentHelper userAgentHelper,
            IRepository<User> userRepository,
            ICauHinhService cauHinhService,
            IRepository<LocaleStringResource> localeStringResourceRepository,
            IRepository<KhoNhanVienQuanLy> khoNhanVienQuanLyRepository,
            IRepository<KyDuTruMuaDuocPhamVatTu> kyDuTruMuaDuocPhamVatTuRepository,
            IRepository<Core.Domain.Entities.VatTus.VatTu> vatTuRepository,
            IRepository<Core.Domain.Entities.VatTuBenhViens.VatTuBenhVien> vatTuBenhVienRepository,
            IRepository<NhapKhoVatTu> nhapKhoVatTuRepository,
            IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository,
            IRepository<Kho> khoRepository,
            IRepository<HopDongThauVatTu> hopDongThauVatTuRepository,
            IRepository<HopDongThauVatTuChiTiet> hopDongThauVatTuChiTietRepository,
            IRepository<DuTruMuaVatTuChiTiet> duTruMuaVatTuChiTietRepository,
            IRepository<Core.Domain.Entities.NhomVatTus.NhomVatTu> nhomVatTuRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
            IRepository<DuTruMuaVatTuTheoKhoa> duTruMuaVatTuTheoKhoaRepository,
            IRepository<DuTruMuaVatTuTheoKhoaChiTiet> duTruMuaVatTuTheoKhoaChiTietRepository,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<DuTruMuaVatTuKhoDuoc> duTruMuaVatTuKhoDuocRepository,
            IRepository<DuTruMuaVatTuKhoDuocChiTiet> duTruMuaVatTuKhoDuocChiTietRepository,
            IRepository<Core.Domain.Entities.DuTruMuaDuocPhams.DuTruMuaDuocPham> duTruMuaDuocPhamRepository,
            IRepository<Core.Domain.Entities.DuTruMuaDuocPhams.DuTruMuaDuocPhamChiTiet> duTruMuaDuocPhamChiTietRepository,
            IRepository<DuTruMuaDuocPhamTheoKhoa> duTruMuaDuocPhamTheoKhoaRepository,
            IRepository<DuTruMuaDuocPhamTheoKhoaChiTiet> duTruMuaDuocPhamTheoKhoaChiTietRepository,
            IRepository<DuTruMuaDuocPhamKhoDuocChiTiet> duTruMuaDuocPhamKhoDuocChiTietRepository,
            IRepository<DuTruMuaDuocPhamKhoDuoc> duTruMuaDuocPhamKhoDuocRepo
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
            _vatTuRepository = vatTuRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _nhapKhoVatTuRepository = nhapKhoVatTuRepository;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _khoRepository = khoRepository;
            _hopDongThauVatTuRepository = hopDongThauVatTuRepository;
            _hopDongThauVatTuChiTietRepository = hopDongThauVatTuChiTietRepository;
            _duTruMuaVatTuChiTietRepository = duTruMuaVatTuChiTietRepository;
            _nhomVatTuRepository = nhomVatTuRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _duTruMuaVatTuTheoKhoaRepository = duTruMuaVatTuTheoKhoaRepository;
            _duTruMuaVatTuTheoKhoaChiTietRepository = duTruMuaVatTuTheoKhoaChiTietRepository;
            _nhanVienRepository = nhanVienRepository;
            _duTruMuaVatTuKhoDuocRepository = duTruMuaVatTuKhoDuocRepository;
            _duTruMuaVatTuKhoDuocChiTietRepository = duTruMuaVatTuKhoDuocChiTietRepository;

            _duTruMuaDuocPhamRepository = duTruMuaDuocPhamRepository;
            _duTruMuaDuocPhamChiTietRepository = duTruMuaDuocPhamChiTietRepository;

            _duTruMuaDuocPhamTheoKhoaRepository = duTruMuaDuocPhamTheoKhoaRepository;
            _duTruMuaDuocPhamTheoKhoaChiTietRepository = duTruMuaDuocPhamTheoKhoaChiTietRepository;
            _duTruMuaDuocPhamKhoDuocChiTietRepository = duTruMuaDuocPhamKhoDuocChiTietRepository;
            _duTruMuaDuocPhamKhoDuocRepo = duTruMuaDuocPhamKhoDuocRepo;
        }       
    }
}
