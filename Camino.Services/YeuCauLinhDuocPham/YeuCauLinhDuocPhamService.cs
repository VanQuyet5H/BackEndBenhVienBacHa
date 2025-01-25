using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Data;
using Camino.Services.Helpers;
using Camino.Core.Domain.Entities.Users;
using Camino.Services.CauHinh;
using Camino.Services.Localization;
using Camino.Services.YeuCauKhamBenh;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.MayXetNghiems;

namespace Camino.Services.YeuCauLinhDuocPham
{
    [ScopedDependency(ServiceType = typeof(IYeuCauLinhDuocPhamService))]
    public partial class YeuCauLinhDuocPhamService : MasterFileService<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham>, IYeuCauLinhDuocPhamService
    {
        private readonly IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> _khoRepository;
        private readonly IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> _khoNhanVienQuanLyRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPhamChiTiet> _yeuCauLinhDuocPhamChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.NhapKhoDuocPhams.NhapKhoDuocPham> _nhapKhoDuocPhamRepository;
        private readonly IRepository<Core.Domain.Entities.XuatKhos.XuatKhoDuocPham> _xuatKhoDuocPhamRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Core.Domain.Entities.NhapKhoDuocPhamChiTiets.NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.Thuocs.DuocPham> _duocPhamRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhVienRepository;
        private readonly IRepository<Core.Domain.Entities.Localization.LocaleStringResource> _localeStringResourceRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham> _yeuCauLinhDuocPhamRepository;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> _khoaPhongNhanVienRepository;
        private readonly IRepository<DuocPhamBenhVienMayXetNghiem> _duocPhamBenhVienMayXetNghiemRepository;
        private readonly IRepository<MayXetNghiem> _mayXetNghiemRepository;
        
        private readonly ICauHinhService _cauHinhService;
        IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        IRepository<Template> _templateRepo;
        IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> _yeuCauKhamBenhRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> _khoaPhongRepository;
        private readonly IRepository<Core.Domain.Entities.CauHinhs.CauHinh> _cauHinhRepository;
        public YeuCauLinhDuocPhamService(
            IRepository<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham> repository,
            IRepository<Core.Domain.Entities.KhoNhanVienQuanLys.KhoNhanVienQuanLy> khoNhanVienQuanLyRepository,
            IRepository<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPhamChiTiet> yeuCauLinhDuocPhamChiTietRepository,
            IRepository<Core.Domain.Entities.NhapKhoDuocPhams.NhapKhoDuocPham> nhapKhoDuocPhamRepository,
            IRepository<Core.Domain.Entities.XuatKhos.XuatKhoDuocPham> xuatKhoDuocPhamRepository,
            IRepository<Core.Domain.Entities.KhoDuocPhams.Kho> khoRepository,
            IRepository<Core.Domain.Entities.Users.User> userRepository,
            IRepository<Core.Domain.Entities.NhapKhoDuocPhamChiTiets.NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository,
            IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository,
            IRepository<Core.Domain.Entities.Thuocs.DuocPham> duocPhamRepository,
            IRepository<Core.Domain.Template> templateRepository, 
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
            IUserAgentHelper userAgentHelper,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauDuocPhamBenhVien> yeuCauDuocPhamBenhVienRepository,
            IRepository<Core.Domain.Entities.Localization.LocaleStringResource> localeStringResourceRepository,
            IRepository<Template> templateRepo,
            ILocalizationService localizationService,
            IRepository<Core.Domain.Entities.YeuCauLinhDuocPhams.YeuCauLinhDuocPham> yeuCauLinhDuocPhamRepository,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh> yeuCauKhamBenhRepository,
            ICauHinhService cauHinhService, IRepository<DuocPhamBenhVienMayXetNghiem> duocPhamBenhVienMayXetNghiemRepository,
            IRepository<Core.Domain.Entities.KhoaPhongNhanViens.KhoaPhongNhanVien> khoaPhongNhanVienRepository,
            IRepository<Core.Domain.Entities.KhoaPhongs.KhoaPhong> khoaPhongRepository,
            IRepository<Core.Domain.Entities.CauHinhs.CauHinh> cauHinhRepository,
            IRepository<MayXetNghiem> mayXetNghiemRepository
            ) : base(repository)
        {
            _khoNhanVienQuanLyRepository = khoNhanVienQuanLyRepository;
            _yeuCauLinhDuocPhamChiTietRepository = yeuCauLinhDuocPhamChiTietRepository;
            _nhapKhoDuocPhamRepository = nhapKhoDuocPhamRepository;
            _xuatKhoDuocPhamRepository = xuatKhoDuocPhamRepository;
            _userAgentHelper = userAgentHelper;
            _khoRepository = khoRepository;
            _userRepository = userRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _duocPhamRepository = duocPhamRepository;
            _templateRepository = templateRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _templateRepo = templateRepo;
            _yeuCauDuocPhamBenhVienRepository = yeuCauDuocPhamBenhVienRepository;
            _localeStringResourceRepository = localeStringResourceRepository;
            _localizationService = localizationService;
            _yeuCauLinhDuocPhamRepository = yeuCauLinhDuocPhamRepository;
            _nhanVienRepository = nhanVienRepository;
            _yeuCauKhamBenhRepository = yeuCauKhamBenhRepository;
            _khoaPhongNhanVienRepository = khoaPhongNhanVienRepository;
            _cauHinhService = cauHinhService;
            _khoaPhongRepository = khoaPhongRepository;
            _cauHinhRepository = cauHinhRepository;
            _duocPhamBenhVienMayXetNghiemRepository = duocPhamBenhVienMayXetNghiemRepository;
            _mayXetNghiemRepository = mayXetNghiemRepository;
        }
    }
}
