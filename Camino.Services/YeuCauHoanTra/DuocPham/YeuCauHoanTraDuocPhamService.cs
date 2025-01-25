using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Data;
using System.Threading.Tasks;
using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.YeuCauHoanTra;
using Camino.Core.Domain.Entities.DuocPhamBenhVienPhanNhoms;
using Camino.Services.Helpers;
using Camino.Services.CauHinh;
using Camino.Services.Localization;

namespace Camino.Services.YeuCauHoanTra.DuocPham
{
    [ScopedDependency(ServiceType = typeof(IYeuCauHoanTraDuocPhamService))]
    public partial class YeuCauHoanTraDuocPhamService : MasterFileService<YeuCauTraDuocPham>, IYeuCauHoanTraDuocPhamService
    {
        private IRepository<YeuCauTraDuocPhamChiTiet> _ycTraDpChiTiet;

        private IRepository<YeuCauNhapKhoDuocPhamChiTiet> _yeuCauNhapKhoDuocPhamChiTietRepository;
        private IRepository<Camino.Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> _duocPhamBenhVienRepository;

        private IRepository<DuocPhamBenhVienPhanNhom> _duocPhamBenhVienPhanNhomRepository;
        private readonly IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> _phongBenhVienRepository;
        private readonly ILocalizationService _localizationService;
        private IRepository<NhapKhoDuocPhamChiTiet> _nhapKhoDuocPhamChiTietRepository;
        private IRepository<Kho> _khoRepository;
        private readonly ICauHinhService _cauHinhService;
        private new readonly IUserAgentHelper _userAgentHelper;
        public YeuCauHoanTraDuocPhamService
            (IRepository<YeuCauTraDuocPham> repository
            , IRepository<YeuCauNhapKhoDuocPhamChiTiet> yeuCauNhapKhoDuocPhamChiTietRepository, IRepository<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTietRepository
            , IRepository<Camino.Core.Domain.Entities.DuocPhamBenhViens.DuocPhamBenhVien> duocPhamBenhVienRepository
            , IRepository<DuocPhamBenhVienPhanNhom> duocPhamBenhVienPhanNhomRepository
            , IUserAgentHelper userAgentHelper
            , IRepository<YeuCauTraDuocPhamChiTiet> ycTraDpChiTiet
            , IRepository<Kho> khoRepository
            , ICauHinhService cauHinhService
            , ILocalizationService localizationService
            , IRepository<Core.Domain.Entities.PhongBenhViens.PhongBenhVien> phongBenhVienRepository) : base(repository)
        {
            _ycTraDpChiTiet = ycTraDpChiTiet;
            _phongBenhVienRepository = phongBenhVienRepository;
            _yeuCauNhapKhoDuocPhamChiTietRepository = yeuCauNhapKhoDuocPhamChiTietRepository;
            _duocPhamBenhVienRepository = duocPhamBenhVienRepository;
            _nhapKhoDuocPhamChiTietRepository = nhapKhoDuocPhamChiTietRepository;
            _khoRepository = khoRepository;
            _duocPhamBenhVienPhanNhomRepository = duocPhamBenhVienPhanNhomRepository;
            _userAgentHelper = userAgentHelper;
            _cauHinhService = cauHinhService;
            _localizationService = localizationService;
        }
    }
}
