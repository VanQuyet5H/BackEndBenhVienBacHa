using Camino.Services.ExportImport;
using Camino.Services.Localization;
using Camino.Services.XuatKhos;
using Camino.Services.YeuCauHoanTra.VatTu;

namespace Camino.Api.Controllers
{
    public partial class YeuCauTraVatTuController : CaminoBaseController
    {
        private readonly IYeuCauHoanTraVatTuService _ycHoanTraVatTuService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IXuatKhoService _xuatKhoService;


        public YeuCauTraVatTuController(
            IYeuCauHoanTraVatTuService ycHoanTraVatTuService,
            ILocalizationService localizationService,
            IExcelService excelService,
            IXuatKhoService xuatKhoService
            )
        {
            _localizationService = localizationService;
            _excelService = excelService;
            _ycHoanTraVatTuService = ycHoanTraVatTuService;
            _xuatKhoService = xuatKhoService;
        }
    }
}
