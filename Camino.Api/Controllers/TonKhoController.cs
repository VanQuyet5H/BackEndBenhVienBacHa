using Camino.Api.Infrastructure.Auth;
using Camino.Services.ExportImport;
using Camino.Services.TonKhos;

namespace Camino.Api.Controllers
{
    public partial class TonKhoController  : CaminoBaseController
    {
        private ITonKhoService _tonKhoService;
        private readonly IExcelService _excelService;

        public TonKhoController(
            IJwtFactory iJwtFactory,
            ITonKhoService tonKhoService,
            IExcelService excelService
        )
        {
            _tonKhoService = tonKhoService;
            _excelService = excelService;
        }
    }
}