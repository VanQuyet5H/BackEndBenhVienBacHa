using System;
using Camino.Services.Localization;
using Camino.Services.YeuCauLinhDuocPham;
using Camino.Api.Infrastructure.Auth;
using Camino.Services.Helpers;
using System.Linq;
using Camino.Services.KhamBenhs;
using Camino.Services.NhapKhoDuocPhams;
using Camino.Services.Users;
using Camino.Services.ExportImport;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhDuocPhamController : CaminoBaseController
    {
        private readonly IYeuCauLinhDuocPhamService _yeuCauLinhDuocPhamService;
        private readonly IYeuCauDuocPhamBenhVienService _yeuCauDuocPhamBenhVienService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IUserService _userService;
        private readonly INhapKhoDuocPhamService _nhapKhoDuocPhamService;
        private readonly IExcelService _excelService;

        public YeuCauLinhDuocPhamController(
            IYeuCauLinhDuocPhamService yeuCauLinhDuocPhamService,
            ILocalizationService localizationService, 
            IJwtFactory iJwtFactory,
            IUserAgentHelper userAgentHelper,
            IYeuCauDuocPhamBenhVienService yeuCauDuocPhamBenhVienService,
            IUserService userService,
            INhapKhoDuocPhamService nhapKhoDuocPhamService,
            IExcelService excelService
            )
        {
            _yeuCauLinhDuocPhamService = yeuCauLinhDuocPhamService;
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
            _yeuCauDuocPhamBenhVienService = yeuCauDuocPhamBenhVienService;
            _userService = userService;
            _nhapKhoDuocPhamService = nhapKhoDuocPhamService;
            _excelService = excelService;
        }
    }
}