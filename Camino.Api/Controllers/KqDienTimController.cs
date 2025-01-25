using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.ExportImport;
using Camino.Services.KetQuaCanLamSang.KetQuaDienTim;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    public class KqDienTimController : CaminoBaseController
    {
        private readonly IKetQuaDienTimService _kqDienTimService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;


        public KqDienTimController(IKetQuaDienTimService kqDienTimService, ILocalizationService localizationService, IExcelService excelService)
        {
            _kqDienTimService = kqDienTimService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _kqDienTimService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.CanLamSang)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _kqDienTimService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
    }
}
