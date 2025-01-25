using Camino.Services.ExportImport;
using Camino.Services.Localization;
using Camino.Services.XuatKhos;
using Camino.Services.YeuCauHoanTra.KSNK;

namespace Camino.Api.Controllers
{
    public partial class YeuCauTraKSNKController : CaminoBaseController
    {
        private readonly IYeuCauHoanTraKSNKService _ycHoanTraKSNKService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;
        private readonly IXuatKhoService _xuatKhoService;
        private readonly Services.YeuCauHoanTra.DuocPham.IYeuCauHoanTraDuocPhamService _ycHoanTraDuocPhamService;

        public YeuCauTraKSNKController(
            IYeuCauHoanTraKSNKService ycHoanTraKSNKService,
            ILocalizationService localizationService,
            IExcelService excelService,
            Services.YeuCauHoanTra.DuocPham.IYeuCauHoanTraDuocPhamService ycHoanTraDuocPhamService,
            IXuatKhoService xuatKhoService
            )
        {
            _localizationService = localizationService;
            _excelService = excelService;
            _ycHoanTraKSNKService = ycHoanTraKSNKService;
            _xuatKhoService = xuatKhoService;
            _ycHoanTraDuocPhamService = ycHoanTraDuocPhamService;
        }
    }
}
