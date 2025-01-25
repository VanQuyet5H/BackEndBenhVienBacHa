using System;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Auth;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.BaoCaoDoanhThuNhaThuocs;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Services.BaoCaoDoanhThuNhaThuocService;
using Camino.Services.ExportImport;
using Camino.Services.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Camino.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaoCaoDoanhThuNhaThuocController : ControllerBase
    {
        private readonly IBaoCaoDoanhThuNhaThuocService _baoCaoDoanhThuNhaThuocService;
        private readonly ILocalizationService _localizationService;
        private readonly IExcelService _excelService;

        public BaoCaoDoanhThuNhaThuocController(IBaoCaoDoanhThuNhaThuocService baoCaoDoanhThuNhaThuocService, ILocalizationService localizationService, IExcelService excelService)
        {
            _baoCaoDoanhThuNhaThuocService = baoCaoDoanhThuNhaThuocService;
            _localizationService = localizationService;
            _excelService = excelService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetDataForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDoanhThuNhaThuoc)]
        public async Task<ActionResult<GridDataSource>> GetDataForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _baoCaoDoanhThuNhaThuocService.GetDataForGridAsync(queryInfo);
            return Ok(gridData);
        }

        [HttpPost("GetTotalPageForGrid")]
        [ClaimRequirement(Enums.SecurityOperation.View, Enums.DocumentType.BaoCaoDoanhThuNhaThuoc)]
        public async Task<ActionResult<GridDataSource>> GetTotalPageForGridAsync([FromBody]QueryInfo queryInfo)
        {
            var gridData = await _baoCaoDoanhThuNhaThuocService.GetTotalPageForGridAsync(queryInfo);
            return Ok(gridData);
        }


        [HttpPost("ExportBaoCaoDoanhThuNhaThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoDoanhThuNhaThuoc)]
        public async Task<ActionResult> ExportBaoCaoDoanhThuNhaThuocAsync(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoDoanhThuNhaThuocService.GetDataForGridAsync(queryInfo);
            var datas = gridData.Data.Cast<BaoCaoDoanhThuNhaThuocGridVo>().ToList();
            var bytes = _baoCaoDoanhThuNhaThuocService.ExportBaoCaoDoanhThuNhaThuoc(datas, queryInfo);
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=BaoCaoDoanhThuNhaThuoc" + DateTime.Now.Year + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            return new FileContentResult(bytes, "application/vnd.ms-excel");
        }

        [HttpPost("InBaoCaoDoanhThuNhaThuoc")]
        [ClaimRequirement(Enums.SecurityOperation.Process, Enums.DocumentType.BaoCaoDoanhThuNhaThuoc)]
        public async Task<ActionResult<string>> InBaoCaoDoanhThuNhaThuoc(QueryInfo queryInfo)
        {
            queryInfo.Skip = 0;
            queryInfo.Take = Int32.MaxValue;
            var gridData = await _baoCaoDoanhThuNhaThuocService.GetDataForGridAsync(queryInfo);

            var datas = gridData.Data.Cast<BaoCaoDoanhThuNhaThuocGridVo>().ToList();
            return _baoCaoDoanhThuNhaThuocService.InBaoCaoDoanhThuNhaThuoc(datas, queryInfo);
        }
    }
}
