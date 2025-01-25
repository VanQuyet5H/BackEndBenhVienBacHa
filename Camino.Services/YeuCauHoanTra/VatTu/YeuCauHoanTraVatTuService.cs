using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Data;
using Camino.Services.CauHinh;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.PhongBenhVien;

namespace Camino.Services.YeuCauHoanTra.VatTu
{
    [ScopedDependency(ServiceType = typeof(IYeuCauHoanTraVatTuService))]
    public partial class YeuCauHoanTraVatTuService : MasterFileService<YeuCauTraVatTu>, IYeuCauHoanTraVatTuService
    {
        private readonly IRepository<YeuCauTraVatTuChiTiet> _ycTraVtChiTiet;
        private readonly IRepository<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTietRepository;
        private readonly IRepository<Kho> _khoRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly ICauHinhService _cauHinhService;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;

        public YeuCauHoanTraVatTuService
            (IRepository<YeuCauTraVatTu> repository,
            IRepository<YeuCauTraVatTuChiTiet> ycTraVtChiTiet,
            IRepository<NhapKhoVatTuChiTiet> nhapKhoVatTuChiTietRepository,
            ICauHinhService cauHinhService,
            ILocalizationService localizationService,
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
        }
    }
}
