using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.Localization;
using Camino.Services.NhapKhoQuaTangMarketing;
using Camino.Services.Users;
using Camino.Services.XuatKhoQuaTangMarketing;

namespace Camino.Api.Controllers
{
    public partial class MarketingController : CaminoBaseController
    {
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IExcelService _excelService;
        private readonly IXuatKhoQuaTangService _xuatKhoQuaTangService;
        private readonly INhapKhoQuaTangMarketingService _nhapKhoQuaTangService;

        public MarketingController(
          ILocalizationService localizationService,
          IUserService userService,
          IUserAgentHelper userAgentHelper,
          IXuatKhoQuaTangService xuatKhoQuaTangService,
          IExcelService excelService,
          INhapKhoQuaTangMarketingService nhapKhoQuaTangService)
        {
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
            _userService = userService;
            _excelService = excelService;
            _xuatKhoQuaTangService = xuatKhoQuaTangService;
            _nhapKhoQuaTangService = nhapKhoQuaTangService;
        }
    }
}