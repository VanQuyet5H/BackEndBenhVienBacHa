using Camino.Api.Infrastructure.Auth;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.KhamBenhs;
using Camino.Services.Localization;
using Camino.Services.Users;
using Camino.Services.YeuCauLinhDuocPham;
using Camino.Services.YeuCauLinhVatTu;

namespace Camino.Api.Controllers
{
    public partial class YeuCauLinhVatTuController : CaminoBaseController
    {
        private readonly IYeuCauLinhVatTuService _yeuCauLinhVatTuService;
        private readonly IYeuCauLinhDuocPhamService _yeuCauLinhDuocPhamService;
        private readonly IYeuCauVatTuBenhVienService _yeuCauVatTuBenhVienService;
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IExcelService _excelService;
        public YeuCauLinhVatTuController(
          IYeuCauLinhVatTuService yeuCauLinhVatTuService,
          IYeuCauLinhDuocPhamService yeuCauLinhDuocPhamService,
          ILocalizationService localizationService,
          IUserService userService,
          IUserAgentHelper userAgentHelper,
          IYeuCauVatTuBenhVienService yeuCauVatTuBenhVienService, 
          IExcelService excelService)
        {
            _yeuCauLinhVatTuService = yeuCauLinhVatTuService;
            _yeuCauLinhDuocPhamService = yeuCauLinhDuocPhamService;
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
            _userService = userService;
            _yeuCauVatTuBenhVienService = yeuCauVatTuBenhVienService;
            _excelService = excelService;
        }
    }
}
