using Camino.Services.ExportImport;
using Camino.Services.Localization;
using Camino.Services.XuatKhos;
using Camino.Services.YeuCauHoanTra.DuocPham;

namespace Camino.Api.Controllers
{
    public partial class YeuCauTraThuocController : CaminoBaseController
    {
        private readonly IYeuCauHoanTraDuocPhamService _ycHoanTraDuocPhamService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IXuatKhoService _xuatKhoService;

        public YeuCauTraThuocController(IYeuCauHoanTraDuocPhamService ycHoanTraDuocPhamService
            , ILocalizationService localizationService, IExcelService excelService
            , IXuatKhoService xuatKhoService)
        {
            _localizationService = localizationService;
            _excelService = excelService;
            _ycHoanTraDuocPhamService = ycHoanTraDuocPhamService;
            _xuatKhoService = xuatKhoService;
        }
    }
}
