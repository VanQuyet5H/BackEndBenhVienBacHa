using System;
using Camino.Services.Localization;
using Camino.Api.Infrastructure.Auth;
using Camino.Services.Helpers;
using System.Linq;
using Camino.Services.Users;
using Camino.Services.ExportImport;
using Camino.Services.YeuCauMuaDuTruDuocPham;
using Camino.Services.Thuocs;
using Camino.Services.YeuCauKhamBenh;
using Camino.Services.YeuCauDuTruDuocPhamTheoKhoa;
using Camino.Services.YeuCauMuaDuocPhamDuTruTaiKhoaDuoc;

namespace Camino.Api.Controllers
{
    public partial class YeuCauMuaDuocPhamController : CaminoBaseController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IUserService _userService;
        private readonly IExcelService _excelService;
        private readonly IYeuCauMuaDuTruDuocPhamService _yeuCauMuaDuTruDuocPhamService;
        private readonly IDuocPhamService _thuocBenhVienService;
        private readonly IYeuCauKhamBenhService _yeuCauKhamBenhService;

        private readonly IYeuCauDuTruDuocPhamTheoKhoaService _yeuCauMuaDuTruDuocPhamTheoKhoaService;
        private readonly IYeuCauMuaDuocPhamDuTruTaiKhoaDuocService _yeuCauMuaDuocPhamDuTruTaiKhoaDuocService;
        public YeuCauMuaDuocPhamController(
            ILocalizationService localizationService,
            IJwtFactory iJwtFactory,
            IUserAgentHelper userAgentHelper,
            IUserService userService,
            IExcelService excelService,
            IDuocPhamService thuocBenhVienService,
            IYeuCauKhamBenhService yeuCauKhamBenhService,
            IYeuCauMuaDuTruDuocPhamService yeuCauMuaDuTruDuocPhamService,
            IYeuCauDuTruDuocPhamTheoKhoaService yeuCauMuaDuTruDuocPhamTheoKhoaService,
            IYeuCauMuaDuocPhamDuTruTaiKhoaDuocService yeuCauMuaDuocPhamDuTruTaiKhoaDuocService
            )
        {
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
            _userService = userService;
            _excelService = excelService;
            _yeuCauMuaDuTruDuocPhamService = yeuCauMuaDuTruDuocPhamService;
            _thuocBenhVienService = thuocBenhVienService;
            _yeuCauKhamBenhService = yeuCauKhamBenhService;
            _yeuCauMuaDuTruDuocPhamTheoKhoaService = yeuCauMuaDuTruDuocPhamTheoKhoaService;
            _yeuCauMuaDuocPhamDuTruTaiKhoaDuocService = yeuCauMuaDuocPhamDuTruTaiKhoaDuocService;
        }
    }
}