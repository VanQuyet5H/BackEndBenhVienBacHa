using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.ExportImport;
using Camino.Services.KetQuaCanLamSang.KetQuaNoiSoi;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class KqNoiSoiController : CaminoBaseController
    {
        private readonly IKetQuaNoiSoiService _kqNoiSoiService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;


        public KqNoiSoiController(IKetQuaNoiSoiService kqNoiSoiService, ILocalizationService localizationService, IExcelService excelService)
        {
            _kqNoiSoiService = kqNoiSoiService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _kqNoiSoiService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _kqNoiSoiService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
    }
}
