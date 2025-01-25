using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Data;
using Camino.Services.Helpers;
using Camino.Services.CauHinh;
using Camino.Services.Localization;
using Camino.Services.XetNghiems;

namespace Camino.Services.XetNghiem
{
    [ScopedDependency(ServiceType = typeof(IXetNghiemService))]
    public partial class XetNghiemService : PhienXetNghiemBaseService, IXetNghiemService
    {
        private readonly IRepository<PhienXetNghiemChiTiet> _phienXetNghiemChiTietRepository;
        private readonly IRepository<PhieuGoiMauXetNghiem> _phieuGoiMauXetNghiemRepository;
        private readonly IRepository<MauXetNghiem> _mauXetNghiemRepository;
        private readonly IRepository<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTietRepository;
        private readonly IUserAgentHelper _userAgentHelper;
        private IRepository<YeuCauTiepNhan> _yeuCauTiepNhanRepository;
        private IRepository<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuatRepository;
        private ILocalizationService _localizationService;
        private IRepository<YeuCauChayLaiXetNghiem> _yeuCauChayLaiXetNghiemRepository;
        private IRepository<Template> _templateRepository;
        private IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private IRepository<Core.Domain.Entities.NhanViens.NhanVien> _nhanVienRepository;
        private IRepository<HopDongKhamSucKhoe> _hopDongKhamSucKhoeRepository;
        private IRepository<PhienXetNghiem> _phienXetNghiemRepository;

        public XetNghiemService(
            IRepository<PhienXetNghiem> repository,
            IRepository<PhienXetNghiemChiTiet> phienXetNghiemChiTietRepository,
            IRepository<PhieuGoiMauXetNghiem> phieuGoiMauXetNghiemRepository,
            IRepository<MauXetNghiem> mauXetNghiemRepository,
            IRepository<KetQuaXetNghiemChiTiet> ketQuaXetNghiemChiTiet,
            IRepository<YeuCauTiepNhan> yeuCauTiepNhanRepository,
            IRepository<YeuCauDichVuKyThuat> yeuCauDichVuKyThuatRepository,
            ICauHinhService cauHinhService,
            ILocalizationService localizationService,
            IRepository<YeuCauChayLaiXetNghiem> yeuCauChayLaiXetNghiemRepository,
            IUserAgentHelper userAgentHelper,
            IRepository<Template> templateRepository,
            IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository,
            IRepository<Core.Domain.Entities.NhanViens.NhanVien> nhanVienRepository,
            IRepository<HopDongKhamSucKhoe> hopDongKhamSucKhoeRepository,
            IRepository<PhienXetNghiem> phienXetNghiemRepository
        ) : base(repository,userAgentHelper,cauHinhService,localizationService)
        {
            _phienXetNghiemChiTietRepository = phienXetNghiemChiTietRepository;
            _phieuGoiMauXetNghiemRepository = phieuGoiMauXetNghiemRepository;
            _mauXetNghiemRepository = mauXetNghiemRepository;
            _ketQuaXetNghiemChiTietRepository = ketQuaXetNghiemChiTiet;
            _yeuCauTiepNhanRepository = yeuCauTiepNhanRepository;
            _yeuCauDichVuKyThuatRepository = yeuCauDichVuKyThuatRepository;
            _localizationService = localizationService;
            _yeuCauChayLaiXetNghiemRepository = yeuCauChayLaiXetNghiemRepository;
            _userAgentHelper = userAgentHelper;
            _templateRepository = templateRepository;
            _phongBenhVienRepository = phongBenhVienRepository;
            _nhanVienRepository = nhanVienRepository;
            _hopDongKhamSucKhoeRepository = hopDongKhamSucKhoeRepository;
            _phienXetNghiemRepository = phienXetNghiemRepository;
        }
    }
}
