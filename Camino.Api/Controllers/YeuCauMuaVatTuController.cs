using Camino.Api.Infrastructure.Auth;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.Users;
using Camino.Services.VatTu;
using Camino.Services.YeuCauMuaDuTruDuocPham;
using Camino.Services.YeuCauMuaDuTruVatTu;
using Camino.Services.YeuCauMuaDuTruVatTuTheoKhoa;
using Camino.Services.YeuCauMuaVatTuDuTruTaiKhoaDuoc;

namespace Camino.Api.Controllers
{

    public partial class YeuCauMuaVatTuController : CaminoBaseController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IUserService _userService;
        private readonly IExcelService _excelService;
        private readonly IVatTuService _vatTuService;
        private readonly IYeuCauMuaDuTruVatTuService _yeuCauMuaDuTruVatTuService;
        private readonly IYeuCauMuaDuTruVatTuTheoKhoaService _yeuCauMuaDuTruVatTuTheoKhoaService;
        private readonly IYeuCauMuaVatTuDuTruTaiKhoaDuocService _yeuCauMuaVatTuDuTruTaiKhoaDuocService;
        private readonly IYeuCauMuaDuTruDuocPhamService _yeuCauMuaDuTruDuocPhamService;

        public YeuCauMuaVatTuController(
            ILocalizationService localizationService,
            IJwtFactory iJwtFactory,
            IUserAgentHelper userAgentHelper,
            IUserService userService,
            IExcelService excelService,
            IVatTuService vatTuService,
            IYeuCauMuaDuTruVatTuService yeuCauMuaDuTruVatTuService,
            IYeuCauMuaDuTruDuocPhamService yeuCauMuaDuTruDuocPhamService,
            IYeuCauMuaDuTruVatTuTheoKhoaService yeuCauMuaDuTruVatTuTheoKhoaService,
            IYeuCauMuaVatTuDuTruTaiKhoaDuocService yeuCauMuaVatTuDuTruTaiKhoaDuocService
            )
        {
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
            _userService = userService;
            _excelService = excelService;
            _vatTuService = vatTuService;
            _yeuCauMuaDuTruVatTuService = yeuCauMuaDuTruVatTuService;
            _yeuCauMuaDuTruDuocPhamService = yeuCauMuaDuTruDuocPhamService;
            _yeuCauMuaDuTruVatTuTheoKhoaService = yeuCauMuaDuTruVatTuTheoKhoaService;
            _yeuCauMuaVatTuDuTruTaiKhoaDuocService = yeuCauMuaVatTuDuTruTaiKhoaDuocService;
        }
    }
}