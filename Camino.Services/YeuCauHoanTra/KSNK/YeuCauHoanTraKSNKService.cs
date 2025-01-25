using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.NhapKhoDuocPhams;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;

namespace Camino.Services.YeuCauHoanTra.KSNK
{
    [ScopedDependency(ServiceType = typeof(IYeuCauHoanTraKSNKService))]
    public partial class YeuCauHoanTraKSNKService : MasterFileService<YeuCauTraVatTu>, IYeuCauHoanTraKSNKService
    {
        private readonly IRepository<YeuCauTraVatTuChiTiet> _ycTraVtChiTiet;
        private readonly IRepository<YeuCauTraVatTu> _yeuCauTraVatTuRepository;

        private readonly IRepository<YeuCauTraDuocPhamChiTiet> _yeuCauTraDuocPhamChiTiet;
        private readonly IRepository<YeuCauTraDuocPham> _yeuCauTraDuocPhamRepository;

        private readonly IRepository<XuatKhoDuocPhamChiTietViTri> _xuatKhoDuocPhamChiTietViTriRepository;
        private readonly IRepository<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTriRepository;

        private readonly IRepository<NhapKhoVatTu> _nhapKhoVatTuRepository;
        private readonly IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        private readonly IRepository<NhapKhoDuocPham> _nhapKhoDuocPhamRepository;
        private readonly IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;

        private readonly IRepository<Kho> _khoRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly ICauHinhService _cauHinhService;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<XuatKhoVatTuChiTiet> _xuatKhoVatTuChiTietRepository;
        private readonly IRepository<Template> _templateRepository;
        private readonly IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;
        private readonly IRepository<VatTuBenhVien> _vatTuBenhVienRepository;

        public YeuCauHoanTraKSNKService
            (IRepository<YeuCauTraVatTu> repository,
            IRepository<YeuCauTraVatTuChiTiet> ycTraVtChiTiet,
            IRepository<YeuCauTraVatTu> yeuCauTraVatTuRepository,
            IRepository<YeuCauTraDuocPhamChiTiet> yeuCauTraDuocPhamChiTiet,
            IRepository<YeuCauTraDuocPham> yeuCauTraDuocPhamRepository,
            IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository,
            ICauHinhService cauHinhService,
            IRepository<XuatKhoVatTuChiTietViTri> xuatKhoVatTuChiTietViTriRepository,
            IRepository<User> userRepository,
            ILocalizationService localizationService,
            IRepository<NhapKhoVatTu> nhapKhoVatTuRepository,
            IRepository<NhapKhoDuocPham> nhapKhoDuocPhamRepository,
            IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository,
            IRepository<XuatKhoVatTuChiTiet> xuatKhoVatTuChiTietRepository,
            IRepository<Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository,
            IRepository<VatTuBenhVien> vatTuBenhVienRepository,
            IRepository<Template> templateRepository,
            IRepository<XuatKhoDuocPhamChiTietViTri> xuatKhoDuocPhamChiTietViTriRepository,
            IRepository<Kho> khoRepository, IUserAgentHelper userAgentHelper, IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository
            ) : base(repository)
        {
            _ycTraVtChiTiet = ycTraVtChiTiet;
            _nhapKhoVatTuChiTietRepository = nhapKhoVatTuChiTietRepository;
            _khoRepository = khoRepository;
            _userAgentHelper = userAgentHelper;
            _phongBenhVienRepository = phongBenhVienRepository;
            _cauHinhService = cauHinhService;
            _localizationService = localizationService;
            _userRepository = userRepository;
            _xuatKhoVatTuChiTietViTriRepository = xuatKhoVatTuChiTietViTriRepository;
            _nhapKhoVatTuRepository = nhapKhoVatTuRepository;
            _xuatKhoVatTuChiTietRepository = xuatKhoVatTuChiTietRepository;
            _templateRepository = templateRepository;
            _nhapKhoDuocPhamRepository = nhapKhoDuocPhamRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _vatTuBenhVienRepository = vatTuBenhVienRepository;
            _yeuCauTraDuocPhamChiTiet = yeuCauTraDuocPhamChiTiet;
            _yeuCauTraVatTuRepository = yeuCauTraVatTuRepository;
            _yeuCauTraDuocPhamRepository = yeuCauTraDuocPhamRepository;
            _xuatKhoDuocPhamChiTietViTriRepository = xuatKhoDuocPhamChiTietViTriRepository;
        }
    }
}
