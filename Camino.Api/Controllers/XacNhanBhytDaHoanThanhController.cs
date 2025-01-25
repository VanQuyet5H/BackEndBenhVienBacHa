using Camino.Services.ExportImport;
using Camino.Services.XacNhanBhytDaHoanThanh;

namespace Camino.Api.Controllers
{
    public partial class XacNhanBhytDaHoanThanhController : CaminoBaseController
    {
        private readonly IExcelService _excelService;
        private readonly IXacNhanBhytDaHoanThanhListService _xacNhanBhytDaHoanThanhListService;
        private readonly IXacNhanBhytDaHoanThanhDetailedService _xacNhanBhytDaHoanThanhDetailedService;

        public XacNhanBhytDaHoanThanhController
        (
            IXacNhanBhytDaHoanThanhListService xacNhanBhytDaHoanThanhListService,
            IExcelService excelService,
            IXacNhanBhytDaHoanThanhDetailedService xacNhanBhytDaHoanThanhDetailedService
        )
        {
            _excelService = excelService;
            _xacNhanBhytDaHoanThanhListService = xacNhanBhytDaHoanThanhListService;
            _xacNhanBhytDaHoanThanhDetailedService = xacNhanBhytDaHoanThanhDetailedService;
        }
    }
}
