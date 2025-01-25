using Camino.Api.Infrastructure.Auth;
using Camino.Services.Localization;
using Camino.Services.ExportImport;
using Camino.Services.Helpers;
using Camino.Services.ICDs;

namespace Camino.Api.Controllers
{
    public partial class QuanLyICDController : CaminoBaseController
    {
        private readonly ILocalizationService _localizationService;
        private readonly IUserAgentHelper _userAgentHelper;
        private readonly IExcelService _excelService;
        private readonly IICDService _icdService;

        public QuanLyICDController(
            ILocalizationService localizationService,
            IJwtFactory iJwtFactory,
            IUserAgentHelper userAgentHelper,
            IExcelService excelService,
            IICDService icdService
            )
        {
            _localizationService = localizationService;
            _userAgentHelper = userAgentHelper;
            _excelService = excelService;
            _icdService = icdService;
        }
    }
}