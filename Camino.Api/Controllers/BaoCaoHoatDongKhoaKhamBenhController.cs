using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.BaoCaoHoatDongKhoaKhamBenhService;
using Camino.Services.ExportImport;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaoCaoHoatDongKhoaKhamBenhController : ControllerBase
    {
        private readonly IBaoCaoHoatDongKhoaKhamBenhService _baoCaoHoatDongKhoaKhamBenhService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public BaoCaoHoatDongKhoaKhamBenhController(IBaoCaoHoatDongKhoaKhamBenhService baoCaoHoatDongKhoaKhamBenhService, ILocalizationService localizationService, IExcelService excelService)
        {
            _baoCaoHoatDongKhoaKhamBenhService = baoCaoHoatDongKhoaKhamBenhService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoHoatDongKhoaKhamBenh)]
        public ActionResult<GridDataSource> GetDataForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = _baoCaoHoatDongKhoaKhamBenhService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGridAsync")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoHoatDongKhoaKhamBenh)]
        public ActionResult<GridDataSource> GetTotalPageForGridAsync
            ([FromBody]QueryInfo queryInfo)
        {
            var gridData = _baoCaoHoatDongKhoaKhamBenhService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }
    }
}
